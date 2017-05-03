using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using System.Web.Http.ModelBinding.Binders;
using System.Web.Http.Validation;
using System.Web.Http.Validation.Providers;
using System.Web.Http.ValueProviders.Providers;
using Abstractor.Cqrs.Infrastructure.CrossCuttingConcerns;
using Abstractor.Cqrs.Infrastructure.Domain;

namespace Abstractor.Owin.WebApi
{
    /// <summary>
    ///     Media type formatter for GenericFile binding from multipart/form-data post.
    /// </summary>
    public class GenericFileMediaTypeFormatter : MediaTypeFormatter
    {
        /// <summary>
        ///     Constructs the media type formatter for multipart/form-data.
        /// </summary>
        public GenericFileMediaTypeFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("multipart/form-data"));
        }

        /// <inheritdoc />
        public override bool CanReadType(Type type)
        {
            Guard.ArgumentIsNotNull(type, nameof(type));
            return true;
        }

        /// <inheritdoc />
        public override bool CanWriteType(Type type)
        {
            Guard.ArgumentIsNotNull(type, nameof(type));
            return false;
        }

        /// <inheritdoc />
        public override async Task<object> ReadFromStreamAsync(
            Type type,
            Stream readStream,
            HttpContent content,
            IFormatterLogger formatterLogger)
        {
            Guard.ArgumentIsNotNull(type, nameof(type));
            Guard.ArgumentIsNotNull(readStream, nameof(readStream));

            try
            {
                var multipartProvider = await content.ReadAsMultipartAsync();
                var modelDictionary = await ToModelDictionaryAsync(multipartProvider);

                return BindToModel(modelDictionary, type, formatterLogger);
            }
            catch (Exception e)
            {
                if (formatterLogger == null) throw;

                formatterLogger.LogError(string.Empty, e);
                return GetDefaultValueForType(type);
            }
        }

        private object BindToModel(IDictionary<string, object> data, Type type, IFormatterLogger formatterLogger)
        {
            Guard.ArgumentIsNotNull(data, nameof(data));
            Guard.ArgumentIsNotNull(type, nameof(type));

            using (var config = new HttpConfiguration())
            {
                // if there is a requiredMemberSelector set, use this one by replacing the validator provider
                var validateRequiredMembers = RequiredMemberSelector != null && formatterLogger != null;

                if (validateRequiredMembers)
                    config.Services.Replace(
                        typeof(ModelValidatorProvider),
                        new RequiredMemberModelValidatorProvider(RequiredMemberSelector));

                // create an action context for model binding
                var actionContext = new HttpActionContext
                {
                    ControllerContext = new HttpControllerContext
                    {
                        Configuration = config,
                        ControllerDescriptor = new HttpControllerDescriptor
                        {
                            Configuration = config
                        }
                    }
                };

                // create model binder context 
                var valueProvider = new NameValuePairsValueProvider(data, CultureInfo.InvariantCulture);
                var metadataProvider = actionContext.ControllerContext.Configuration.Services
                                                    .GetModelMetadataProvider();
                var metadata = metadataProvider.GetMetadataForType(null, type);
                var modelBindingContext = new ModelBindingContext
                {
                    ModelName = string.Empty,
                    FallbackToEmptyPrefix = false,
                    ModelMetadata = metadata,
                    ModelState = actionContext.ModelState,
                    ValueProvider = valueProvider
                };

                // bind model 
                var modelBinderProvider = new CompositeModelBinderProvider(config.Services.GetModelBinderProviders());
                var binder = modelBinderProvider.GetBinder(config, type);
                var haveResult = binder.BindModel(actionContext, modelBindingContext);

                if (formatterLogger == null)
                    return haveResult
                        ? modelBindingContext.Model
                        : GetDefaultValueForType(type);

                // log validation errors 
                foreach (var modelStatePair in actionContext.ModelState)
                foreach (var modelError in modelStatePair.Value.Errors)
                    formatterLogger.LogError(modelStatePair.Key, modelError.ErrorMessage);

                return haveResult
                    ? modelBindingContext.Model
                    : GetDefaultValueForType(type);
            }
        }

        private static async Task<IDictionary<string, object>> ToModelDictionaryAsync(
            MultipartStreamProvider multipartProvider)
        {
            var dictionary = new Dictionary<string, object>();

            foreach (var part in multipartProvider.Contents)
            {
                var name = part.Headers.ContentDisposition.Name.Trim('"');

                dictionary[name] = !string.IsNullOrEmpty(part.Headers.ContentDisposition.FileName)
                    ? (object) (part.Headers.ContentLength.GetValueOrDefault() > 0
                        ? new GenericFile(
                            part.Headers.ContentDisposition.FileName.Trim('"'),
                            await part.ReadAsStreamAsync())
                        : null)
                    : await part.ReadAsStringAsync();
            }

            return dictionary;
        }
    }
}