using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel.Web;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace Herschel.Ws
{
    public class DynamicResponseFormatAttribute : Attribute, IOperationBehavior
    {
        public void AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
        {
        }

        public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
        {
            dispatchOperation.Formatter = new DynamicResponseMessageFormatter(
                dispatchOperation.Formatter,
                new Dictionary<string, IDispatchMessageFormatter>()
                {
                        {"text/plain", new TextResponseMessageFormatter() }
                });
        }

        public void Validate(OperationDescription operationDescription)
        {
        }
    }
}