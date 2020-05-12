using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace mvc.Security
{
    public class EmailConfirmationTokenProvider<TUser> : DataProtectorTokenProvider<TUser> where TUser : class
    {
        private readonly ILogger<DataProtectorTokenProvider<TUser>> logger;

        public EmailConfirmationTokenProvider(IDataProtectionProvider dataProtectionProvider, 
                                              IOptions<EmailConfirmationTokenProviderOptions> options, 
                                              ILogger<DataProtectorTokenProvider<TUser>> logger) : base(dataProtectionProvider, options, logger)
        {
            this.logger = logger;
        }
    }

    public class EmailConfirmationTokenProviderOptions : DataProtectionTokenProviderOptions
    {
        
    }

}