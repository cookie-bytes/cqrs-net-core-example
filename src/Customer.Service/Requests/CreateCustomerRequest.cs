using System;
using System.Collections.Generic;

namespace CustomerCommandService.Requests
{
    public class CreateCustomerRequest
    {
        public long Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }        
    }
}