using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ShoppingStore.Domain.Entities;
using ShoppingStore.Application.Contracts;
using ShoppingStore.Domain.Common.Extensions;
using ShoppingStore.Application.ViewModels.Comments;

namespace ShoppingStore.Application.Services
{
    public class InsertCommentUser : IInsertCommentUser
    {
        private readonly IMapper _mapper;
        private readonly IDataBaseContext _context;
        public InsertCommentUser(
            IMapper mapper,
            IDataBaseContext context)
        {
            _context = context;
            _mapper = mapper;
            _mapper.CheckArgumentIsNull(nameof(_mapper));
        }

        public async Task SendComment(string name, string email, string description, string productId, int? parentCommentId)
        {
            var viewModel = new CommentViewModel();
            int maxId;
            var comments = await _context.Comments.AsNoTracking().ToListAsync();
            if (comments.Count() != 0)
                maxId = comments.OrderByDescending(c => c.Id).First().Id + 1;
            else
                maxId = 1;
            viewModel.Id = maxId;
            viewModel.Name = name;
            viewModel.Email = email;
            viewModel.ProductId = productId;
            viewModel.Description = description;
            viewModel.ParentId = viewModel.ParentCommentId;
            viewModel.ParentCommentId = parentCommentId == 0 ? null : parentCommentId;
            viewModel.InsertTime = DateTimeExtensions.DateTimeWithOutMilliSecends(DateTime.Now);
            await _context.Comments.AddAsync(_mapper.Map<Comment>(viewModel));
            await _context.SaveChangesAsync();
        }
    }
}
