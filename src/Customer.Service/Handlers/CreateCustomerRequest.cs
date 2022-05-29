using MediatR;
namespace CustomerCommandService.Handlers
{
    public class CreateCustomerRequest : IRequest<CreateCustomerResponse>
    {
        public long Id{get;set;}
        public string FirstName{get;set;}
        public string LastName{get;set;}
    }
}
 
 