﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using RestfulASPNetCore.Web.Dtos;
using RestfulASPNetCore.Web.Entities;
using RestfulASPNetCore.Web.Helpers;
using RestfulASPNetCore.Web.Services;
using System.Linq;


namespace RestfulASPNetCore.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(
                setup =>
                {
                    setup.ReturnHttpNotAcceptable = true;
                    setup.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
                    setup.InputFormatters.Add(new XmlDataContractSerializerInputFormatter(new MvcOptions { }));

                    var jsonOutputFormatter = setup.OutputFormatters.OfType<JsonOutputFormatter>().FirstOrDefault();
                    if (jsonOutputFormatter != null)
                    {
                        jsonOutputFormatter.SupportedMediaTypes.Add(VendorMediaType.HateoasMediaType);
                    }

                    //setup.InputFormatters.Add(new XmlDataContractSerializerInputFormatter());
                }
            )
            .AddJsonOptions(options =>
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver()
            )
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            var connectionString = Configuration["connectionStrings:libraryDBConnectionString"];
            services.AddDbContext<LibraryContext>(o => o.UseSqlServer(connectionString));
            services.AddScoped<ILibraryRepository, LibraryRepository>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelper, UrlHelper>(imp =>
               {
                   var actionContext = imp.GetService<IActionContextAccessor>().ActionContext;
                   return new UrlHelper(actionContext);
               }
            );

            services.AddTransient<IPropertyMappingService, PropertyMappingService>();
            services.AddTransient<ITypeHelperService, TypeHelperService>();



        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, LibraryContext libraryContext, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();
            loggerFactory.AddDebug(LogLevel.Trace);

            //.Net core 1.1 approach
            //loggerFactory.AddProvider(new NLog.Extensions.Logging.NLogLoggerProvider());
            //loggerFactory.AddNLog();


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();


            AutoMapper.Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Entities.Author, Dtos.Author>()
                    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                    .ForMember(dest => dest.Age, opt => opt.MapFrom(src =>
                        src.DateOfBirth.GetCurrentAge()));

                cfg.CreateMap<Entities.Book, Dtos.Book>();

                cfg.CreateMap<CreateAuthor, Entities.Author>();

                cfg.CreateMap<CreateBook, Entities.Book>();
                cfg.CreateMap<UpdateBook, Entities.Book>();
                cfg.CreateMap<Entities.Book, UpdateBook>();

            });

            libraryContext.EnsureSeedDataForContext();

            app.UseMvc();
        }
    }
}
