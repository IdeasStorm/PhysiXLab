using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using PhysiXEngine;

namespace PhysiXEngine.Helpers
{
    public class XMLPlayer
    {
        private List<Body> Bodies;
        private XmlTextReader reader;

        public XMLPlayer(List<Body> Bodies, string FilePath)
        {
            this.Bodies = Bodies;
            reader = new XmlTextReader(FilePath);

            // Reading The <Declaration> and <Experiment> Elements
            reader.Read();
            reader.Read();
        }

        /// <summary>
        /// This Method Does the Creation, Deletion, and Updating of all Bodies, the changes affect
        /// bodies in the Bodies List, no need to update individual bodies, this method does that.
        /// </summary>
        public void Update()
        {
            if (!reader.Read())
                return;
            if ((reader.Name == "Experiment"))   // if reached </Experiment>
                return;

            // Now we have a new Cycle
            XmlElement Root = (new XmlDocument()).CreateElement("Root");
            Root.InnerXml = reader.ReadInnerXml();

            XmlNode Removes = Root.ChildNodes[0];
            XmlNode Adds = Root.ChildNodes[1];
            XmlNode Updates = Root.ChildNodes[2];
            if (Removes.HasChildNodes)
                foreach (XmlNode Node in Removes.ChildNodes)
                {
                    Bodies.Remove(GetBodyByGUID(UInt32.Parse(Node.InnerText.Substring(2))));
                }
            if (Adds.HasChildNodes)
                foreach (XmlNode Node in Adds.ChildNodes)
                {
                    Bodies.Add(NewBody(Node.InnerText));
                }
            if (Updates.HasChildNodes)
                foreach (XmlNode Node in Updates.ChildNodes)
                {
                    GetBodyByGUID(UInt32.Parse(Node.Name.Substring(2))).Update(Node.InnerText);
                }

            // No Need to read </Cycle> because ReadInnerXml() already does it.
        }

        /// <summary>
        /// Creates a new body based on the passed string, this method is used in the Update.
        /// Override this method in to add new Body based types, but at the end of the derived
        /// method, return base.NewBody() to manipulate old types.
        /// </summary>
        /// <param name="S">The String used in determining the Body's Type, and passed to its constrcutor</param>
        public virtual Body NewBody(string S)
        {
            string BodyType = S.Split('|')[0];

            if (BodyType.Equals("Sphere"))
                return new Sphere(S);
            else if (BodyType.Equals("Box"))
                return new Box(S);
            else return null;
        }

        private Body GetBodyByGUID(UInt32 GUID)
        {
            for (int i = 0; i < Bodies.Count; i++)
                if (Bodies[i].GUID == GUID)
                    return Bodies[i];
            return null;
        }

        public void Stop()
        {
            reader.Close();
        }

    }
}
