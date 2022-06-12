using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShoppingStore.Domain.Common.Api;
using ShoppingStore.Application.Contracts;
using ShoppingStore.Domain.Common.Extensions;
using ShoppingStore.Domain.Common.Api.Attributes;

namespace ShoppingStore.Web.Areas.Api.Controllers.v1
{
    [DisplayName("CommentApi"), ApiResultFilter, ApiVersion("1"), Route("api/v{version:apiVersion}/[controller]")]
    public class CommentApiController : ControllerBase
    {
        private readonly IUnitOfWork _uw;
        public CommentApiController(IUnitOfWork uw)
        {
            _uw = uw;
            _uw.CheckArgumentIsNull(nameof(_uw));
        }

        [HttpPost]
        public async Task<ApiResult<string>> SendComment(string name, string email, string description, string productId, int? parentCommentId)
        {
            if (name.HasValue() && email.HasValue() && description.HasValue())
            {
                await _uw.InsertCommentUser.SendComment(name, email, description, productId, parentCommentId);
                return Ok("با تشکر از بیان نظر ,دیدگاه شما بعد از تایید در سایت نمایش داده می شود.");
            }
            return Ok();
        }

        [HttpGet]
        public async Task<ApiResult<object>> LikeOrDisLike(int commentId, string browserId, bool isLiked)
        {
            await _uw.InsertLikeOrDislike.LikeOrDisLike(commentId, browserId, isLiked);
            var likeAndDislike = _uw.SiteComments.NumberOfLikeAndDislike(commentId);
            return new { like = likeAndDislike.NumberOfLike, dislike = likeAndDislike.NumberOfDisLike };
        }
    }
}
