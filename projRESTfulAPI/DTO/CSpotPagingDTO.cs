using projRESTfulAPI.Models;
using System.Collections;

namespace projRESTfulAPI.DTO
{
    public class CSpotPagingDTO
    {
        public List<SpotImagesSpot>? spotsResult { get; set; }
        public int totalPages { get; set; }
        public int totalCount { get; set; }
    }
}
