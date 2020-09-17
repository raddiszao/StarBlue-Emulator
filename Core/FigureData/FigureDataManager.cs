using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace StarBlue.Core.FigureData
{
    public class FigureDataManager
    {
        private static readonly ILog log = LogManager.GetLogger("StarBlue.Core.FigureData");
        private Dictionary<string, Dictionary<string, Figure>> FigureParts;

        public FigureDataManager()
        {
            FigureParts = new Dictionary<string, Dictionary<string, Figure>>();
        }

        public void Init()
        {
            if (FigureParts.Count > 0)
            {
                FigureParts.Clear();
            }

            try
            {
                XDocument Doc = XDocument.Load(@"./config/figuredata.xml");

                var data = (from item in Doc.Descendants("sets") from tItem in Doc.Descendants("settype") select new { Part = tItem.Elements("set"), Type = tItem.Attribute("type"), });
                foreach (var item in data.ToList())
                {
                    foreach (var part in item.Part.ToList())
                    {
                        string PartName = item.Type.Value;
                        if (!FigureParts.ContainsKey(PartName))
                        {
                            FigureParts.Add(PartName, new Dictionary<string, Figure>());
                        }

                        if (!FigureParts[PartName].ContainsKey(part.Attribute("id").Value))
                        {
                            FigureParts[PartName].Add(part.Attribute("id").Value, new Figure(PartName, part.Attribute("id").Value, part.Attribute("gender").Value, part.Attribute("colorable").Value));
                        }
                    }
                }
            }
            catch (XmlException e)
            {
                Logging.LogException("Figure part xml not found." + e.ToString());
            }
            catch (Exception e)
            {
                Logging.LogException(e.ToString());
            }

            log.Info(">> Loaded " + FigureParts.Count + " Figure Parts.");
        }

        public string ProcessFigure(string Figure, string Gender)
        {
            List<string> FigureLook = new List<string>();
            List<string> FigureParts = new List<string>();
            string[] RequiredParts = { "hd" };
            bool ForDefault = false;

            foreach (string Part in Figure.Split('.').ToList())
            {
                string NewPart = Part;
                string[] PartsValue = Part.Split('-');
                if (PartsValue.Count() < 2)
                {
                    ForDefault = true;
                    continue;
                }

                string PartName = PartsValue[0];
                string PartId = PartsValue[1];

                if (!this.FigureParts.ContainsKey(PartName) || !this.FigureParts[PartName].ContainsKey(PartId) || (Gender != "U" && this.FigureParts[PartName][PartId].Gender != "U" && this.FigureParts[PartName][PartId].Gender != Gender))
                {
                    NewPart = SetDefault(PartName, Gender);
                }

                if (!FigureParts.Contains(PartName))
                {
                    FigureParts.Add(PartName);
                }

                if (!FigureLook.Contains(NewPart))
                {
                    FigureLook.Add(NewPart);
                }
            }

            if (ForDefault)
            {
                FigureLook.Clear();
                FigureLook.AddRange("sh-3338-93.ea-1406-62.hr-831-49.ha-3331-92.hd-180-7.ch-3334-93-1408.lg-3337-92.ca-1813-62".Split('.'));
            }

            foreach (string RequiredPart in RequiredParts.Where(requiredPart => !FigureParts.Contains(requiredPart) && !FigureLook.Contains(SetDefault(requiredPart, Gender))))
            {
                FigureLook.Add(SetDefault(RequiredPart, Gender));
            }

            return string.Join(".", FigureLook);
        }

        private string SetDefault(string PartName, string Gender)
        {
            string PartId = "0";
            if (FigureParts.ContainsKey(PartName))
            {
                KeyValuePair<string, Figure> Part = FigureParts[PartName].FirstOrDefault(x => x.Value.Gender == Gender || Gender == "U");
                PartId = Part.Equals(default(KeyValuePair<string, Figure>)) ? "0" : Part.Key;
            }

            return PartName + "-" + PartId + "-1";
        }
    }

    class Figure
    {
        private readonly string Part;
        private readonly string PartId;
        public string Gender;
        private readonly string Colorable;
        public Figure(string part, string partId, string gender, string colorable)
        {
            Part = part;
            PartId = partId;
            Gender = gender;
            Colorable = colorable;
        }
    }

}