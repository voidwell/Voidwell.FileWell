using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Voidwell.FileWell.Data;
using Voidwell.Cache;
using Voidwell.FileWell.Services;
using Voidwell.Logging;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.HttpOverrides;
using System.Threading.Tasks;

namespace Voidwell.FileWell
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath);

            if (env.IsDevelopment())
            {
                builder.AddJsonFile("devsettings.json", true, true);
            }

            builder.AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore()
                .AddDataAnnotations()
                .AddJsonFormatters(options =>
                { 
                    options.NullValueHandling = NullValueHandling.Ignore;
                    options.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                });
            services.AddMvc();

            services.AddOptions();
            services.Configure<AppOptions>(Configuration);

            services.AddEntityFrameworkContext(Configuration);
            services.AddCache(Configuration, "Voidwell.FileWell");

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            var appOptions = Configuration.Get<AppOptions>();

            services.AddAuthentication("oidc")
                .AddCookie("Cookies", o => o.Cookie.Name = "voidwell")
                .AddOpenIdConnect("oidc", options =>
                {
                    options.SignInScheme = "Cookies";

                    options.Authority = appOptions.Authority;
                    options.RequireHttpsMetadata = true;

                    options.ClientId = "voidwell-filewell";
                    options.ClientSecret = appOptions.ClientSecret;

                    options.ResponseType = "code id_token";

                    options.SignedOutRedirectUri = appOptions.SignoutRedirectUrl;

                    options.GetClaimsFromUserInfoEndpoint = true;

                    options.Scope.Add("openid");
                    options.Scope.Add("profile");
                    options.Scope.Add("voidwell-roles");

                    options.TokenValidationParameters.NameClaimType = "name";
                    options.TokenValidationParameters.RoleClaimType = "role";

                    options.ClaimActions.Add(new JsonKeyArrayClaimAction("role", "role", "role"));

                    options.Events.OnRedirectToIdentityProvider = async context =>
                    {
                        context.ProtocolMessage.RedirectUri = $"{appOptions.CallbackHost}/signin-oidc";
                        await Task.FromResult(0);
                    };
                    options.Events.OnRedirectToIdentityProviderForSignOut = async context =>
                    {
                        context.ProtocolMessage.PostLogoutRedirectUri = $"{appOptions.CallbackHost}/signout-callback-oidc";
                        await Task.FromResult(0);
                    };
                });

            services.AddTransient<IFileService, FileService>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseForwardedHeaders(GetForwardedHeaderOptions());

            app.UseLoggingMiddleware();

            app.UseAuthentication();

            app.UseStaticFiles();

            app.UseMvc();
        }

        private static ForwardedHeadersOptions GetForwardedHeaderOptions()
        {
            var options = new ForwardedHeadersOptions
            {
                RequireHeaderSymmetry = false,
                ForwardLimit = 15,
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
            };

            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();

            return options;
        }
    }
}
