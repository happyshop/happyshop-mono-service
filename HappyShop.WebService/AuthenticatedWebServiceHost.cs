using System;
using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.ServiceModel.Web;

namespace HappyShop.WebService
{
  public class AuthenticatedWebServiceHost : WebServiceHost
  {
    public AuthenticatedWebServiceHost(Type type, Uri url)
    {
      InitializeDescription(type, new UriSchemeKeyedCollection());
      IDictionary<string, ContractDescription> desc;
      base.CreateDescription(out desc);
      var val = desc.Values.First();

      WebHttpBinding binding = new WebHttpBinding();
      binding.Security.Mode = WebHttpSecurityMode.TransportCredentialOnly;
      binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;

      Credentials.UserNameAuthentication.UserNamePasswordValidationMode = UserNamePasswordValidationMode.Custom;
      Credentials.UserNameAuthentication.CustomUserNamePasswordValidator = new CustomUserNamePasswordValidator();

      AddServiceEndpoint(val.ContractType, binding, url);
    }

    //Possible next question:
    //"How can I get the name of the authenticated user?"
    public static string UserName
    {
      get
      {
        if( OperationContext.Current == null || OperationContext.Current.ServiceSecurityContext == null || OperationContext.Current.ServiceSecurityContext.PrimaryIdentity == null)
        {
          return null;
        }
        return OperationContext.Current.ServiceSecurityContext.PrimaryIdentity.Name;
      }
    }

    public class CustomUserNamePasswordValidator : UserNamePasswordValidator
    {
      public override void Validate(string userName, string password)
      {
        if (userName != password)
        {
          throw new NotSupportedException("Username or password wrong.");
        }
      }
    }
  }
}
