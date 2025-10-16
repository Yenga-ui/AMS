using AssetManagementSystem.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace AssetManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly SqlConnection _connection;

        public OrdersController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        }

        [HttpGet("search")]
        public async Task<IActionResult> GetOrdersByLast4Digits([FromQuery] string digits)
        {
            if (string.IsNullOrWhiteSpace(digits) || digits.Length != 4 || !int.TryParse(digits, out _))
            {
                return BadRequest("Invalid input. Must be exactly 4 digits.");
            }

            var results = new List<string>();
            string query = @"
        SELECT DISTINCT PORDER.POHNUM_0 AS OrderNumber
        FROM [192.168.105.198\SAGESQL].[x3data].[TANDEM].PORDER
        LEFT JOIN [192.168.105.198\SAGESQL].[x3data].[TANDEM].[BPARTNER] B ON PORDER.BPSNUM_0 = B.BPRNUM_0
        LEFT JOIN [192.168.105.198\SAGESQL].[x3data].[TANDEM].[PORDERP] ON PORDERP.POHNUM_0 = PORDER.POHNUM_0
        LEFT JOIN [192.168.105.198\SAGESQL].[x3data].[TANDEM].CPTANALIN dim ON dim.VCRNUM_0 = PORDER.POHNUM_0 AND dim.VCRLIN_0 = PORDERP.POPLIN_0
        LEFT JOIN [192.168.105.198\SAGESQL].[x3data].[TANDEM].[PORDERQ] ON PORDER.POHNUM_0 = PORDERQ.POHNUM_0 AND PORDERP.POPLIN_0 = PORDERQ.POPLIN_0
        LEFT JOIN [192.168.105.198\SAGESQL].[x3data].TANDEM.CACCE dim1 ON dim1.DIE_0 = dim.DIE_0 AND dim1.CCE_0 = dim.CCE_0
        LEFT JOIN [192.168.105.198\SAGESQL].[x3data].TANDEM.CACCE dim2 ON dim2.DIE_0 = dim.DIE_1 AND dim2.CCE_0 = dim.CCE_1
        LEFT JOIN [192.168.105.198\SAGESQL].[x3data].TANDEM.CACCE dim3 ON dim3.DIE_0 = dim.DIE_2 AND dim3.CCE_0 = dim.CCE_2
        LEFT JOIN [192.168.105.198\SAGESQL].[x3data].TANDEM.CACCE dim4 ON dim4.DIE_0 = dim.DIE_3 AND dim4.CCE_0 = dim.CCE_3
        LEFT JOIN [192.168.105.198\SAGESQL].[x3data].TANDEM.CACCE dim5 ON dim5.DIE_0 = dim.DIE_4 AND dim5.CCE_0 = dim.CCE_4
        LEFT JOIN [192.168.105.198\SAGESQL].[x3data].[TANDEM].[TABCUR] ON TABCUR.CUR_0 = PORDER.CUR_0
        WHERE RIGHT(PORDER.POHNUM_0, 4) = @digits 
        ORDER BY PORDER.POHNUM_0";

            Console.WriteLine(query);
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@digits", digits);
                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            results.Add(reader.GetString(0));
                        }
                    }
                }

                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{orderNumber}")]
        public async Task<IActionResult> GetDimension(string orderNumber)
        {
            try
            {
                var dimension = await GetDimensionByOrderNumber(orderNumber);

                if (dimension == null)
                    return NotFound();

                return Ok(dimension);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private async Task<Dimension?> GetDimensionByOrderNumber(string orderNumber)
        {
            string sql = @"SELECT DISTINCT PORDER.POHNUM_0 AS PO_NUMBER,
                              PORDER.POHFCY_0 AS SITE_CODE,
                              dim.CCE_0 AS REQ_DIMENSION1_CODE,
                              dim.CCE_1 AS REQ_DIMENSION2_CODE,
                              dim.CCE_2 AS REQ_DIMENSION3_CODE,
                              dim.CCE_3 AS REQ_DIMENSION4_CODE,
                              QTYPUU_0 AS QTY,
                              GROPRI_0 AS UNIT_PRICE,
                              '' AS STOCK_CODE,
                              CONCAT(ITMDES1_0, ITMDES_0) AS STOCK_DESC,
                              PORDER.CUR_0 AS STOCK_CAT_CODE,
                              CURDES_0 AS STOCK_CAT_DESC,
                              PORDERP.POPLIN_0
                       FROM [192.168.105.198\SAGESQL].[x3data].[TANDEM].PORDER
                       LEFT JOIN [192.168.105.198\SAGESQL].[x3data].[TANDEM].[PORDERP] ON PORDERP.POHNUM_0 = PORDER.POHNUM_0
                       LEFT JOIN [192.168.105.198\SAGESQL].[x3data].[TANDEM].CPTANALIN dim ON dim.VCRNUM_0 = PORDER.POHNUM_0 AND dim.VCRLIN_0 = PORDERP.POPLIN_0
                       LEFT JOIN [192.168.105.198\SAGESQL].[x3data].[TANDEM].[TABCUR] ON TABCUR.CUR_0 = PORDER.CUR_0
                       WHERE PORDER.POHNUM_0 = @OrderNumber 
                       ORDER BY PORDER.POHNUM_0";

            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await conn.OpenAsync();

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@OrderNumber", orderNumber);

            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Dimension
                {
                    PO_NUMBER = reader["PO_NUMBER"]?.ToString(),
                    SITE_CODE = reader["SITE_CODE"]?.ToString(),
                    REQ_DIMENSION1_CODE = reader["REQ_DIMENSION1_CODE"]?.ToString(),
                    REQ_DIMENSION2_CODE = reader["REQ_DIMENSION2_CODE"]?.ToString(),
                    REQ_DIMENSION3_CODE = reader["REQ_DIMENSION3_CODE"]?.ToString(),
                    REQ_DIMENSION4_CODE = reader["REQ_DIMENSION4_CODE"]?.ToString(),
                   
                };
            }

            return null;
        }
    }
}

