/* add global usage directives for the System namespaces here */

global using System.Reflection;
global using System.Net;
global using System.Net.Http.Headers;
global using System.Text;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using System.Net.Http.Json;
global using System.Linq.Expressions;
global using System.Security.Claims;


/* add global usage directives for the Microsoft namespaces here */

global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Hosting;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Mvc.Testing;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.EntityFrameworkCore;

/* add global usage directives for application namespaces here */

global using Comanda.WebApi;
global using Comanda.WebApi.Data;
global using Comanda.WebApi.Data.Repositories;
global using Comanda.WebApi.Extensions;
global using Comanda.WebApi.Entities;
global using Comanda.WebApi.Handlers;
global using Comanda.WebApi.Exceptions;
global using Comanda.WebApi.Services;
global using Comanda.WebApi.Services.Exceptions;
global using Comanda.WebApi.Payloads;
global using Comanda.WebApi.Validators;
global using Comanda.WebApi.Helpers;
global using Comanda.WebApi.Controllers;
global using Comanda.TestingSuite.Fixtures;

/* add global usage directives for third-party namespaces here */

global using AutoFixture;
global using Moq;
global using Xunit;