namespace PCSetupHub.Data.CsvModels
{
	public interface ICsvConvertible<T>
	{
		T ConvertToModel();
	}
}