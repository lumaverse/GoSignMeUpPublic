using System.Threading.Tasks;

public interface IRestService<TRestModel>
{
    Task<TRestModel> ReadObject();

}