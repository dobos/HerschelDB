using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.IO;

namespace Herschel.Ws.Api
{
    public class TextResponseMessageFormatter : IDispatchMessageFormatter
    {
        public void DeserializeRequest(Message message, object[] parameters)
        {
            throw new NotImplementedException();
        }

        public Message SerializeReply(MessageVersion messageVersion, object[] parameters, object result)
        {
            return WebOperationContext.Current.CreateStreamResponse(new TextResponseMessageBodyWriter(result), "text/plain");
        }
    }
}