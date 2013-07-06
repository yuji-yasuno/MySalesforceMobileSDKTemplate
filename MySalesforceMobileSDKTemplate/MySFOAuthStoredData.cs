using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace MySalesforceMobileSDKTemplate
{
    [DataContract]
    class MySFOAuthStoredData
    {
        [DataMember]
        public String accessToken = "";
        [DataMember]
        public String refreshToken = "";
        [DataMember]
        public String instanceUrl = "";
    }
}
