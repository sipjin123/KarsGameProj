using UnityEngine;
using System.Collections;

using Common.Xml;

public class XmlTest : MonoBehaviour {

	[SerializeField]
	private TextAsset sampleXml;

	void Awake() {
		Assertion.AssertNotNull(sampleXml, "sampleXml");

		SimpleXmlReader reader = new SimpleXmlReader();
		reader.PrintXML(reader.Read(sampleXml.text), 0);
	}

}
