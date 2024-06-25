/* add global usage directives for System namespaces here */

global using System.Reflection;
global using System.Linq.Expressions;
global using System.Text;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using System.Security.Claims;
global using System.IdentityModel.Tokens.Jwt;

/* add global usage directives for Microsoft namespaces here */

global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
global using Microsoft.OpenApi.Models;
global using Microsoft.Extensions.Diagnostics.HealthChecks;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.IdentityModel.Tokens;

/* add global usage directives for Comanda namespaces here */

global using Comanda.WebApi.Entities;
global using Comanda.WebApi.Data;
global using Comanda.WebApi.Data.Repositories;
global using Comanda.WebApi.Payloads;
global using Comanda.WebApi.Handlers;
global using Comanda.WebApi.Services;
global using Comanda.WebApi.Services.Exceptions;
global using Comanda.WebApi.Validators;
global using Comanda.WebApi.Utils;
global using Comanda.WebApi.Middlewares;
global using Comanda.WebApi.Extensions;

/* add global usage directives for Third party namespaces here */

global using MediatR;
global using FluentValidation;
global using Nelibur.ObjectMapper;