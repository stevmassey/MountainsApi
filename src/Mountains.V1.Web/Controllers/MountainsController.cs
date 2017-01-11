﻿using Mountains.ServiceModels;
using Mountains.V1.Client.Dtos;
using Mountains.V1.Web.DataMappers;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Mountains.V1.Web.Controllers
{
    public sealed class MountainsController : BaseController
    {
        public MountainsController(IMountainService mountainService)
        {
            _mountainService = mountainService;
        }

        [HttpGet]
        [Route("mountains")]
        public MountainCollectionDto Get()
        {
            return new MountainCollectionDto { Mountains = _mountainService.GetMountains().Select(MountainMapper.Map) };
        }

        [HttpGet]
        [Route("mountains/{id}")]
        public MountainDto Get(string id)
        {
            Mountain mountain = _mountainService.GetMountain(ParseId(id));

            if (mountain == null)
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, "Cannot find mountain"));

            return MountainMapper.Map(mountain);
        }

        [HttpPost]
        [Route("mountains")]
        public MountainDto Post([FromBody]MountainDto mountainDto)
        {
            if (mountainDto == null)
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Must supply mountain"));

            Mountain mountain = _mountainService.AddMountain(MountainMapper.Map(mountainDto));

            return MountainMapper.Map(mountain);
        }

        [HttpPut]
        [Route("mountains/{id}")]
        public MountainDto Put(string id, [FromBody]MountainDto mountainDto)
        {
            if (mountainDto == null)
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Must supply mountain"));

            Mountain oldMountain = _mountainService.GetMountain(ParseId(id));

            if (oldMountain == null)
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, "Cannot find mountain"));

            Mountain newMountain = _mountainService.UpdateMountain(ParseId(id), MountainMapper.Map(mountainDto));

            return MountainMapper.Map(newMountain);
        }

        [HttpDelete]
        [Route("mountains/{id}")]
        public void Delete(string id)
        {
            Mountain mountain = _mountainService.GetMountain(ParseId(id));

            if (mountain == null)
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, "Cannot find mountain"));

            _mountainService.DeleteMountain(ParseId(id));
        }

        private IMountainService _mountainService;
    }
}