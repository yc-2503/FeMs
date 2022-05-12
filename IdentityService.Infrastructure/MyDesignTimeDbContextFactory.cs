using IdentityService.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

//这个类仅仅用在开发阶段生成数据库模型，也是没有办法，因为在WebAPI中会在注入的时候配置User，EFCore一直报错说生命周期不对
class MyDesignTimeDbContextFactory : IDesignTimeDbContextFactory<AuDbcontex>
{
	public AuDbcontex CreateDbContext(string[] args)
	{
		DbContextOptionsBuilder<AuDbcontex> builder = new();

		builder.UseSqlite("Data Source=../IdentityService.WebAPI/Identity.db");
		return new AuDbcontex(builder.Options);
	}
}