using UnityEngine;
using System.Collections.Generic;

namespace Common.Xml {

	public class SimpleXmlReader {
	
	    private const char TAG_START = '<';
	    private const char TAG_END = '>';
	    private const char SPACE = ' ';
	    private const char QUOTE = '"';
	    private const char SLASH = '/';
	    private const char EQUALS = '=';
	    private const char EXCLAMATION = '!';
	    private const char QUESTUIN_MARK = '?';
	    private static readonly string BEGIN_QUOTE = "" + EQUALS + QUOTE;
	
	    public SimpleXmlNode Read(string xml) {
	        int lastIndex = 0;
	        SimpleXmlNode rootNode = new SimpleXmlNode();
	        SimpleXmlNode currentNode = rootNode;

			int xmlLength = xml.Length;
	
	        while (true) {
	            int index = xml.IndexOf(TAG_START, lastIndex);
	
				if (index < 0 || index >= xmlLength) break;
	
	            index++;
	
	            lastIndex = xml.IndexOf(TAG_END, index);
				if (lastIndex < 0 || lastIndex >= xmlLength) break;
	
	            int tagLength = lastIndex - index;
	            string xmlTag = xml.Substring(index, tagLength);
	
	            // if the tag starts with a </ then it is an end tag
	            if (xmlTag[0] == SLASH){
	                currentNode = currentNode.ParentNode;
	                continue;
	            }
	
	            bool openTag = true;
	
	            // if the tag starts with <! or <? it's a comment
	            if (xmlTag[0] == EXCLAMATION || xmlTag[0] == QUESTUIN_MARK) 
	                openTag = false;
	
	            // if the tag ends in /> the tag can be considered closed
	            if (xmlTag[tagLength - 1] == SLASH) {
	                // cut away the slash
	                xmlTag = xmlTag.Substring(0, tagLength - 1);
	                openTag = false;
	            }
	
	            SimpleXmlNode node = ParseTag(xmlTag);
	            node.ParentNode = currentNode;
	            currentNode.Children.Add(node);
	
	            if (openTag) currentNode = node;
	        }
	        return rootNode;
	    }
	
	    public SimpleXmlNode ParseTag(string xmlTag) {
	        SimpleXmlNode node = new SimpleXmlNode();
	
	        int nameEnd = xmlTag.IndexOf(SPACE, 0);
	        if (nameEnd < 0) {
	            node.TagName = xmlTag;
	            return node;
	        }
	
	        string tagName = xmlTag.Substring(0, nameEnd);
	        node.TagName = tagName;
	
	        string attrString = xmlTag.Substring(nameEnd, xmlTag.Length - nameEnd);
	        return ParseAttributes(attrString, node);
	    }
	
	    public SimpleXmlNode ParseAttributes(string xmlTag, SimpleXmlNode node) {
	
	        int lastIndex = 0;
	
	        while (true) {
	            int index = xmlTag.IndexOf(BEGIN_QUOTE, lastIndex);
	            if (index < 0 || index > xmlTag.Length) break;
	
	            int attrNameIndex = xmlTag.LastIndexOf(SPACE, index);
	            if (attrNameIndex < 0 || attrNameIndex > xmlTag.Length) break;
	
	            attrNameIndex++;
	            string attrName = xmlTag.Substring(attrNameIndex, index - attrNameIndex);
	
	            // skip the equal and quote character
	            index += 2;
	
	            lastIndex = xmlTag.IndexOf(QUOTE, index);
	            if (lastIndex < 0 || lastIndex > xmlTag.Length) break;
	
	            int tagLength = lastIndex - index;
	            string attrValue = xmlTag.Substring(index, tagLength);
	
	            node.Attributes[attrName] = attrValue;
	        }
	
	        return node;
	    }
	
	    public void PrintXML(SimpleXmlNode node, int indent) {
	        indent++;
	
	        foreach (SimpleXmlNode n in node.Children) {
	            string attr = " ";
	            foreach (KeyValuePair<string, string> p in n.Attributes)
	                attr += "[" + p.Key + ": " + p.Value + "] ";
	
	            string indentString = "";
	            for (int i = 0; i < indent; i++)
	                indentString += "/";
	
	            Debug.Log(indentString + " " + n.TagName + attr);
	            PrintXML(n, indent);
	        }
	    }
	}
}

