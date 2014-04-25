using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel.Channels;
using System.ServiceModel;
using System.Net;
using System.ServiceModel.Dispatcher;

namespace Herschel.Ws
{
    public class DynamicResponseMessageFormatter : IDispatchMessageFormatter
    {
        private IDispatchMessageFormatter fallbackFormatter;
        private Dictionary<string, IDispatchMessageFormatter> formatters;

        public Dictionary<string, IDispatchMessageFormatter> Formatters
        {
            get { return formatters; }
        }

        public DynamicResponseMessageFormatter(IDispatchMessageFormatter fallbackFormatter, IDictionary<string, IDispatchMessageFormatter> formatters)
        {
            this.fallbackFormatter = fallbackFormatter;
            this.formatters = new Dictionary<string,IDispatchMessageFormatter>(formatters, StringComparer.InvariantCultureIgnoreCase);
        }

        public void DeserializeRequest(Message message, object[] parameters)
        {
            fallbackFormatter.DeserializeRequest(message, parameters);
        }

        public Message SerializeReply(MessageVersion messageVersion, object[] parameters, object result)
        {
            var accepts = GetAcceptedContentTypes(OperationContext.Current.RequestContext.RequestMessage);
            
            IDispatchMessageFormatter formatter = null;

            if (accepts != null)
            {
                for (int i = 0; i < accepts.Length; i++)
                {
                    if (accepts[i] != null && formatters.ContainsKey(accepts[i]))
                    {
                        formatter = formatters[accepts[i]];
                        break;
                    }
                }
            }

            if (formatter == null)
            {
                formatter = fallbackFormatter;
            }

            return formatter.SerializeReply(messageVersion, parameters, result);
        }

        private string[] GetAcceptedContentTypes(Message request)
        {
            var prop = (HttpRequestMessageProperty)request.Properties[HttpRequestMessageProperty.Name];
            var accepts = prop.Headers[HttpRequestHeader.Accept];

            if (accepts == null)
            {
                accepts = prop.Headers[HttpRequestHeader.ContentType];
            }

            if (accepts == null)
            {
                return null;
            }
            else
            {
                return accepts.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            }
        }
    }
}