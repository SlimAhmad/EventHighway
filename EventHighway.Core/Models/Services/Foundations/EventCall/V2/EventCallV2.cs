// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using EventHighway.Core.Models.Services.Foundations.HandlerConfigurations;
using EventHighway.Core.Models.Services.Foundations.PromotedProperties;

namespace EventHighway.Core.Models.Services.Foundations.EventCall.V2
{
    public class EventCallV2
    {
        public Guid HandlerId { get; set; }
        public string HandlerName { get; set; }
        public List<HandlerConfiguration> HandlerConfigurations { get; set; } = new();
        public string Content { get; set; }
        public IEnumerable<string> RequiredPromotedProperties { get; set; } = new List<string>();
        public List<PromotedProperty> PromotedProperties { get; set; } = new();
        public string FilterCriteria { get; set; }
        public string Response { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public bool IsSuccess { get; set; }
    }
}
