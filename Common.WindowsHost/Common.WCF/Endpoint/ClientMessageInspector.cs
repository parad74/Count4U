using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.Xml.XPath;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.ServiceModel;

namespace Common.WCF
{
  public class ClientMessageInspector : IClientMessageInspector, IDispatchMessageInspector
    {

        #region IClientMessageInspector Members
        public void AfterReceiveReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
        {
			
          //  Console.WriteLine("=============AfterReceiveReply =======================");
			System.Diagnostics.Debug.Print("=============AfterReceiveReply =======================");

			//XDocumentResponse = TransformMessageCopy(reply);

			//Console.WriteLine("reply: {0}", reply);
			System.Diagnostics.Debug.Print("[" + DateTime.Now + "] reply :");
			System.Diagnostics.Debug.Print(String.Format("{0}", reply));

			//Console.ReadKey();
        }

        public object BeforeSendRequest(ref System.ServiceModel.Channels.Message request, System.ServiceModel.IClientChannel channel)
        {
		
			//Console.WriteLine("====SimpleMessageInspector+BeforeSendRequest is called=====");
			System.Diagnostics.Debug.Print("=============SimpleMessageInspector+BeforeSendRequest is called =======================");

			//Console.WriteLine("request: {0}", request);
			System.Diagnostics.Debug.Print("[" + DateTime.Now + "] request :");
			System.Diagnostics.Debug.Print(String.Format("{0}", request));

			////modify the request send from client(only customize message body)
			//request = TransformMessage2(request);

            //may modify the entire message
            //request = TransformMessage(request);

            return null;
        }

		//helper method
		//reformat the entire  message
		private XDocument TransformMessageCopy(Message oldMessage)
		{
			//Message newMessage = null;
			MessageBuffer msgbuf = oldMessage.CreateBufferedCopy(int.MaxValue);
			XPathNavigator nav = msgbuf.CreateNavigator();


			//load the old message into xmldocument
			MemoryStream ms = new MemoryStream();
			XmlWriter xw = XmlWriter.Create(ms);
			nav.WriteSubtree(xw);
			xw.Flush();
			xw.Close();

			ms.Position = 0;
			XDocument xdoc = XDocument.Load(
				XmlReader.Create(ms)
				);

			return xdoc;
		}

        //helper method
        //reformat the entire  message
        private Message TransformMessage(Message oldMessage)
        {
            Message newMessage = null;
            MessageBuffer msgbuf = oldMessage.CreateBufferedCopy(int.MaxValue);
            XPathNavigator nav = msgbuf.CreateNavigator();
            

            //load the old message into xmldocument
            MemoryStream ms = new MemoryStream();
            XmlWriter xw = XmlWriter.Create(ms);
            nav.WriteSubtree(xw);
            xw.Flush();
            xw.Close();

            ms.Position = 0;
            XDocument xdoc = XDocument.Load(
                XmlReader.Create(ms)
                );


            //perform transformation
			var strElms = xdoc.Descendants(XName.Get("StringValue", "urn:test:datacontracts"));
			foreach (XElement strElm in strElms) strElm.Value = "[Modified in SimpleMessageInspector]" + strElm.Value;

            
            xw = XmlWriter.Create(ms);
            ms.Position= 0;
            xdoc.Save(
               xw
                );
            xw.Flush();
            xw.Close();

            ms.Position = 0;
            StreamReader sr = new StreamReader(ms);
            Console.WriteLine(sr.ReadToEnd());
   


            //create the new message
            ms.Position = 0;
            XmlDictionaryReader xdr = XmlDictionaryReader.CreateTextReader(
                ms, new XmlDictionaryReaderQuotas()
                );
            newMessage = Message.CreateMessage(xdr, int.MaxValue, oldMessage.Version );


            return newMessage;
        }

        //only read and modify the Message Body part
        private Message TransformMessage2(Message oldMessage)
        {
            Console.WriteLine("oldMessage: {0}", oldMessage);
            Message newMessage = null;

            //load the old message into XML
            MessageBuffer msgbuf = oldMessage.CreateBufferedCopy(int.MaxValue);

            Message tmpMessage = msgbuf.CreateMessage();
            XmlDictionaryReader xdr = tmpMessage.GetReaderAtBodyContents();

            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(xdr);
            xdr.Close();

            
            //transform the xmldocument
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xdoc.NameTable);
            nsmgr.AddNamespace("a", "urn:test:datacontracts");

            XmlNode node = xdoc.SelectSingleNode("//a:StringValue", nsmgr);
            if(node!= null) node.InnerText = "[Modified in SimpleMessageInspector]" + node.InnerText;


            MemoryStream ms = new MemoryStream();
            XmlWriter xw = XmlWriter.Create(ms);
            xdoc.Save(xw);
            xw.Flush();
            xw.Close();

            ms.Position = 0;
            XmlReader xr = XmlReader.Create(ms);

            
            //create new message from modified XML document
            newMessage = Message.CreateMessage(oldMessage.Version, null,xr );
            newMessage.Headers.CopyHeadersFrom(oldMessage);
            newMessage.Properties.CopyProperties(oldMessage.Properties);

            return newMessage;
        }

        #endregion



        #region IDispatchMessageInspector Members

        public object AfterReceiveRequest(ref System.ServiceModel.Channels.Message request, System.ServiceModel.IClientChannel channel, System.ServiceModel.InstanceContext instanceContext)
        {
              return null;
        }

        public void BeforeSendReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
        {
        }

        #endregion
    }
}