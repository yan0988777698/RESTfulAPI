using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using projRESTfulAPI.DTO;
using projRESTfulAPI.Models;

namespace projRESTfulAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpotsController : ControllerBase
    {
        MyDbContext _dbContext;
        public SpotsController(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SpotImagesSpot>>> GetSpots()
        {
            return await _dbContext.SpotImagesSpots.ToListAsync();
        }
        [HttpGet]
        [Route("search")]
        public async Task<ActionResult<IEnumerable<SpotImagesSpot>>> GetSpots(string keyword)
        {
            return await _dbContext.SpotImagesSpots.Where(s => s.SpotTitle.Contains(keyword)).ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<CSpotPagingDTO>> GetSpot([FromBody] CSpotDTO cSpotDTO)
        {
            var spots = cSpotDTO.categoryId == 0 ?
                _dbContext.SpotImagesSpots :
                _dbContext.SpotImagesSpots.Where(x => x.CategoryId == cSpotDTO.categoryId);
            if (!string.IsNullOrEmpty(cSpotDTO.keyword))
            {
                spots = spots.Where(
                    x => x.SpotTitle.Contains(cSpotDTO.keyword) ||
                    x.SpotDescription.Contains(cSpotDTO.keyword));
            }
            int totalCount = spots.Count();
            int pageSize = cSpotDTO.pageSize;
            int page = cSpotDTO.page;
            int totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);
            spots = spots.Skip((page - 1) * pageSize).Take(pageSize);
            switch (cSpotDTO.sortBy)
            {
                case "spotTitle":
                    spots = cSpotDTO.sortType == "asc" ? spots.OrderBy(x => x.SpotTitle) : spots.OrderByDescending(x => x.SpotTitle);
                    break;
                default:
                    spots = cSpotDTO.sortType == "asc" ? spots.OrderBy(x => x.SpotId) : spots.OrderByDescending(x => x.SpotId);
                    break;
            }

            CSpotPagingDTO pagingDTO = new CSpotPagingDTO();
            pagingDTO.totalPages = totalPages;
            pagingDTO.totalCount = totalCount;
            pagingDTO.spotsResult = await spots.ToListAsync();
            return pagingDTO;
        }
    }

}
