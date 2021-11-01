using System.Collections.Generic;

namespace TPaper.DeliveryRequest
{
  public class Response : ValueObject
  {
    public string Status { get; private set; }

    private Response()
    {

    }

    public Response(string status)
    {
      Status = status;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
      yield return Status;
    }
  }
}
