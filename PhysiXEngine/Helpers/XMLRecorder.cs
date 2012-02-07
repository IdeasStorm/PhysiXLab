using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using PhysiXEngine;

namespace PhysiXEngine.Helpers
{
    public class XMLRecorder
    {
        private class Pair
        {
            public bool Found;
            public UInt32 GUID;
            public Pair(bool Found, UInt32 GUID)
            {
                this.Found = Found;
                this.GUID = GUID;
            }
        }

        private XmlTextWriter writer;
        private List<Body> Bodies;
        private Int64 LastGUID = -1;
        private List<Pair> LastCycleGUIDs;

        public XMLRecorder(List<Body> Bodies, string FilePath)
        {
            writer = new XmlTextWriter(FilePath, null);
            XmlDocument Doc = new XmlDocument();
            Doc.CreateXmlDeclaration("1.0", string.Empty, string.Empty).WriteTo(writer);
            writer.WriteStartElement("Experiment");

            this.Bodies = Bodies;
            LastCycleGUIDs = new List<Pair>(Bodies.Count);
            for (int i = 0; i < LastCycleGUIDs.Count; i++)
            {
                LastCycleGUIDs.Add(new Pair(false, Bodies[i].GUID));
                if (LastGUID < Bodies[i].GUID)
                    LastGUID = Bodies[i].GUID;
            }
        }

        public void Update()
        {
            // Reset this list to be able to know the deleted items
            for (int i = 0; i < LastCycleGUIDs.Count; i++)
                LastCycleGUIDs[i].Found = false;

            XmlDocument Doc = new XmlDocument();
            Int64 NewLastGUID = LastGUID;
            XmlElement Cycle = Doc.CreateElement("Cycle");
            XmlElement Adds = Doc.CreateElement("Adds");
            XmlElement Removes = Doc.CreateElement("Removes");
            XmlElement Updates = Doc.CreateElement("Updates");
            foreach (Body B in Bodies)
            {
                XmlElement NewInfo = Doc.CreateElement("ID" + B.GUID.ToString());
                NewInfo.InnerText = B.Position.X.ToString()
                                  + '|' + B.Position.Y.ToString()
                                  + '|' + B.Position.Z.ToString()
                                  + '|' + B.Orientation.W.ToString();
                Updates.AppendChild(NewInfo);
                if (B.GUID > LastGUID)
                {
                    if (B.GUID > NewLastGUID)
                        NewLastGUID = B.GUID;
                    LastCycleGUIDs.Add(new Pair(true, B.GUID));
                    XmlElement elem = Doc.CreateElement("add");
                    elem.InnerText = B.ToString();
                    Adds.AppendChild(elem);
                }
                else
                {
                    for (int i = 0; i < LastCycleGUIDs.Count; i++)
                        if (LastCycleGUIDs[i].GUID == B.GUID)
                        {
                            LastCycleGUIDs[i].Found = true;
                            break;
                        }
                }
            }

            // The GUIDs of removed bodies: add them to the XMLDocument and removes them from LastCycleGUIDs 
            foreach (Pair P in LastCycleGUIDs)
            {
                if (P.Found == false)
                {
                    XmlElement elem = Doc.CreateElement("rem");
                    elem.InnerText = P.GUID.ToString();
                    LastCycleGUIDs.Remove(P);

                }
            }
            LastGUID = NewLastGUID;

            Cycle.AppendChild(Removes);
            Cycle.AppendChild(Adds);
            Cycle.AppendChild(Updates);
            Doc.AppendChild(Cycle);
            Doc.WriteTo(writer);
        }

        /// <summary>
        /// Note: once stopped, The Recorder cannot be resumed.
        /// </summary>
        public void Stop()
        {
            writer.Close();
        }

    }
}