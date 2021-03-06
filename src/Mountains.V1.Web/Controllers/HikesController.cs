﻿using Mountains.ServiceModels;
using Mountains.V1.Client.Dtos;
using Mountains.V1.Web.DataMappers;
using Mountains.V1.Web.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Mountains.V1.Web.Controllers
{
    public sealed class HikesController : BaseController
    {
        public HikesController(AuthenticationService authenticationService, IMountainService mountainService, IUserService userService)
            : base(authenticationService)
        {
            _mountainService = mountainService;
            _userService = userService;
        }

        [HttpGet]
        [Route("hikes")]
        public HikeCollectionDto List(int start = 0, int? count = null, string mountainId = null, string userId = null)
        {
            return new HikeCollectionDto { Hikes = _mountainService.GetHikes(start, GetCount(count), ParseIdOrDefault(mountainId), ParseIdOrDefault(userId)).Select(HikeMapper.Map) };
        }

        [HttpGet]
        [Route("hikes/{id}")]
        public HikeDto Get(string id)
        {
            Hike hike = _mountainService.GetHike(ParseId(id));

            if (hike == null)
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, "Cannot find hike"));

            return HikeMapper.Map(hike);
        }

        [HttpPost]
        [Route("hikes")]
        public HikeDto Post([FromBody]HikeDto hikeDto)
        {
            ValidateHike(hikeDto);

            Hike hike = _mountainService.AddHike(HikeMapper.Map(hikeDto));

            return HikeMapper.Map(hike);
        }

        [HttpDelete]
        [Route("hikes/{id}")]
        public void Delete(string id)
        {
            Hike hike = _mountainService.GetHike(ParseId(id));

            if (hike == null || hike.UserId != AuthenticationService.UserId)
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, "Cannot find hike"));

            _mountainService.DeleteHike(ParseId(id));
        }

        private void ValidateHike(HikeDto hike)
        {
            if (hike == null)
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Must supply hike"));

            if (hike.MountainId == null || _mountainService.GetMountain(ParseId(hike.MountainId)) == null)
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, "Cannot find mountain"));

            if (hike.UserId == null || ParseId(hike.UserId) != AuthenticationService.UserId || _userService.GetUser(ParseId(hike.UserId)) == null)
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, "Cannot find user"));
        }

        private readonly IMountainService _mountainService;
        private readonly IUserService _userService;
    }
}
