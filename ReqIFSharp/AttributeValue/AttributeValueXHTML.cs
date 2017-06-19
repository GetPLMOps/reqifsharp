﻿// -------------------------------------------------------------------------------------------------
// <copyright file="AttributeValueXHTML.cs" company="RHEA System S.A.">
//
//   Copyright 2017 RHEA System S.A.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ReqIFSharp
{
    using System;
    using System.Runtime.Serialization;
    using System.Xml;
    
    /// <summary>
    /// The purpose of the <see cref="AttributeValueXHTML"/> class is to define an attribute value with XHTML contents.
    /// </summary>
    [Serializable]
    public class AttributeValueXHTML : AttributeValue
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeValueXHTML"/> class.
        /// </summary>
        public AttributeValueXHTML()
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeValueXHTML"/> class.
        /// </summary>
        /// <param name="specElAt">
        /// The owning <see cref="SpecElementWithAttributes"/>
        /// </param>
        internal AttributeValueXHTML(SpecElementWithAttributes specElAt)
            : base(specElAt)
        {
        }

        /// <summary>
        /// Gets or sets the XHTML Content
        /// </summary>
        public string TheValue { get; set; }

        /// <summary>
        /// Gets or sets the Linkage to the original attribute value that has been saved if isSimplified is true.
        /// </summary>
        public string TheOriginalValue { get; set; }

        /// <summary>
        /// Gets or sets the Reference to the attribute definition that relates the value to its data type.
        /// </summary>
        public AttributeDefinitionXHTML Definition { get; set; }

        /// <summary>
        /// Gets the <see cref="AttributeDefinition "/>
        /// </summary>
        /// <returns>
        /// An instance of <see cref="AttributeDefinitionXHTML"/>
        /// </returns>
        protected override AttributeDefinition GetAttributeDefinition()
        {
            return this.Definition;
        }

        /// <summary>
        /// Sets the <see cref="AttributeDefinition"/>
        /// </summary>
        /// <param name="attributeDefinition">
        /// The <see cref="AttributeDefinition"/> to set
        /// </param>
        protected override void SetAttributeDefinition(AttributeDefinition attributeDefinition)
        {
            if (attributeDefinition.GetType() != typeof(AttributeDefinitionXHTML))
            {
                throw new ArgumentException("attributeDefinition must of type AttributeDefinitionXHTML");
            }

            this.Definition = (AttributeDefinitionXHTML)attributeDefinition;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the attribute value is a simplified representation of the original value.
        /// </summary>
        public bool IsSimplified { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether <see cref="IsSimplified"/> is specified
        /// </summary>
        public bool IsSimplifiedSpecified { get; set; }

        /// <summary>
        /// Generates a <see cref="AttributeValueXHTML"/> object from its XML representation.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        public override void ReadXml(XmlReader reader)
        {
            var isimplified = reader["IS-SIMPLIFIED"];
            if (!string.IsNullOrEmpty(isimplified))
            {
                this.IsSimplified = bool.Parse(isimplified);
            }

            using (var subtree = reader.ReadSubtree())
            {
                while (subtree.Read())
                {
                    if (subtree.MoveToContent() == XmlNodeType.Element && reader.LocalName == "ATTRIBUTE-DEFINITION-XHTML-REF")
                    {
                        var reference = reader.ReadElementContentAsString();

                        AttributeDefinitionXHTML attributeDefinitionXhtml = null;
                        foreach (var specType in this.SpecElAt.ReqIfContent.SpecTypes)
                        {
                            foreach (var attributeDefinition in specType.SpecAttributes)
                            {
                                if (attributeDefinition.Identifier == reference)
                                {
                                    attributeDefinitionXhtml = (AttributeDefinitionXHTML)attributeDefinition;
                                    break;
                                }
                            }
                        }

                        this.Definition = attributeDefinitionXhtml;
                    }

                    if (subtree.MoveToContent() == XmlNodeType.Element && reader.LocalName == "THE-VALUE")
                    {
                        this.TheValue = subtree.ReadInnerXml().Trim();
                    }
                }
            }
        }

        /// <summary>
        /// Converts a <see cref="AttributeValueXHTML"/> object into its XML representation.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        /// <exception cref="SerializationException">
        /// The <see cref="Definition"/> may not be null
        /// </exception>
        public override void WriteXml(XmlWriter writer)
        {
            if (this.Definition == null)
            {
                throw new SerializationException(string.Format("The Definition property of an AttributeValueXHTML may not be null"));
            }

            writer.WriteAttributeString("IS-SIMPLIFIED", this.IsSimplified.ToString());

            writer.WriteStartElement("DEFINITION");
            writer.WriteElementString("ATTRIBUTE-DEFINITION-XHTML-REF", this.Definition.Identifier);
            writer.WriteEndElement();
   
            writer.WriteStartElement("THE-VALUE");
            writer.WriteRaw(this.TheValue);
            writer.WriteEndElement();

            if (!string.IsNullOrEmpty(this.TheOriginalValue))
            {
                writer.WriteStartElement("THE-ORIGINAL-VALUE");
                writer.WriteRaw(this.TheOriginalValue);
                writer.WriteEndElement();
            }
        }
    }
}