using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remote
{
    public interface ISharedTypeInterface
    {
        string GetRemoteStatus(string stringToPrint);
    }

    public class SharedType : MarshalByRefObject, ISharedTypeInterface
    {
        public string GetRemoteStatus(string stringToPrint)
        {
            string returnStatus = "Ticket Confirmed";
            Console.WriteLine("Enquiry for {0}", stringToPrint);
            Console.WriteLine("Sending back status: {0}", returnStatus);

            return returnStatus;
        }
    }
}
