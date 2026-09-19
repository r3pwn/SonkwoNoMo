using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SonkwoNoMo.Server.Attributes;
using SonkwoNoMo.Server.Config;
using SonkwoNoMo.Server.Models;

namespace SonkwoNoMo.Server.Controllers;

[ApiController]
[Route("/api/")]
public class RegionController(IOptionsMonitor<RegionListOptions> regionOptions) : ControllerBase
{
    [HttpGet("get_region_list")]
    [XorResponse]
    public IActionResult GetList()
    {
        var response = new RegionListResponse
        {
            AccInfo = regionOptions.CurrentValue.AccInfo,
            Regions = regionOptions.CurrentValue.Catalog.Select(region => new Region
            {
                Name = $"{region.Name}|{region.Location}",
                RequestNum = 25 // I'm not sure what this actually does
            }).ToList()
        };

        return Ok(response);
    }
}