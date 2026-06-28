// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.ClientV2.SubstrateApp.Models.MediaItems.Exceptions
{
    public class InvalidMediaItemException : Xeption
    {
        public InvalidMediaItemException(string message)
            : base(message)
        { }
    }
}
