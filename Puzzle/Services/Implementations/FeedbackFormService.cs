using Extensions;
using LocalizingPkg.Shared.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Puzzle.Mapper;
using Puzzle.Repository.Interfaces;
using Puzzle.Services.Interfaces;
using Puzzle.ViewModels.Feedbacks;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Puzzle.Services.Implementations
{
    public class FeedbackFormService : IFeedbackFormService
    {
        #region Fields

        private readonly IFeedbackFormRepository _feedbackFormRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        #endregion

        #region Ctor

        public FeedbackFormService(IFeedbackFormRepository feedbackFormRepository, IHttpContextAccessor httpContextAccessor)
        {
            _feedbackFormRepository = feedbackFormRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        #endregion

        public async Task<Result<FilterFeedbackFormViewModel>> Filter(FilterFeedbackFormViewModel model)
        {
            if (model is null)
            {
                return Result.Failure<FilterFeedbackFormViewModel>(message: "پارامتر های ورودی نمی تواند خالی باشد");
            }

            var query = _feedbackFormRepository
                .Query()
                .Include(a => a.Customer)
                .Include(a => a.Designer)
                .AsQueryable();

            #region Conditions

            if (!string.IsNullOrEmpty(model.DesignerId))
            {
                query = query.Where(x => x.DesignerId == model.DesignerId);
            }

            if (model.CustomerId.HasValue)
            {
                query = query.Where(x => x.CustomerId == model.CustomerId);
            }

            if (model.Vote.HasValue)
            {
                query = query.Where(x => x.Vote == model.Vote);
            }

            query = query.OrderByDescending(a => a.CreatedAt);

            #endregion

            await model.Paging(query.Select(a => a.ToViewModel()));

            return model;
        }

        public async Task<Result<FeedbackFormViewModel>> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                return Result.Failure<FeedbackFormViewModel>(message: "شناسه وارد شده صحیح نمی باشد");
            }

            var feedbackForm = await _feedbackFormRepository
                .Query()
                .Where(a => a.Id == id)
                .Include(a => a.Customer)
                .Include(a => a.Designer)
                .FirstOrDefaultAsync();

            if (feedbackForm is null)
            {
                return Result.Failure<FeedbackFormViewModel>(message: "فرم نظر سنجی با این شناسه یافت نشد");
            }

            return feedbackForm.ToViewModel();
        }

        public async Task<Result> CreateAsync(UpsertFeedbackFormViewModel model)
        {
            if (model is null)
            {
                return Result.Failure(message: "پارامتر های ورودی نمی تواند خالی باشد");
            }

            model.Sanitize();

            try
            {
                var customerId = _httpContextAccessor.HttpContext.User.GetUserId();
                model.CustomerId = int.Parse(customerId);

                var feedbackForm = model.ToModel();

                await _feedbackFormRepository.AddAsync(feedbackForm);
                await _feedbackFormRepository.SaveChangesAsync();

                return Result.Success();
            }

            catch (Exception ex)
            {
                return Result.Failure(message: ex.Message);
            }
        }

        public async Task<Result> DeleteAsync(int id)
        {
            if (id <= 0)
            {
                return Result.Failure(message: "شناسه وارد شده صحیح نمی باشد");
            }

            //Check the login user role is the admin or not
            var isAdmin = _httpContextAccessor.HttpContext.User.IsAdmin();

            if (!isAdmin)
            {
                return Result.Failure(message: "شما دسترسی لازم برای انجام این عملیات را ندارید");
            }

            var feedbackForm = _feedbackFormRepository
                .Query()
                .FirstOrDefault(a => a.Id == id);

            if (feedbackForm is null)
            {
                return Result.Failure(message: "فرم نظر سنجی با این شناسه یافت نشد");
            }

            feedbackForm.IsDeleted = true;
            feedbackForm.UpdatedAt = DateTime.Now;

            _feedbackFormRepository.UpdateAsync(feedbackForm);
            await _feedbackFormRepository.SaveChangesAsync();

            return Result.Success();
        }
    }
}
