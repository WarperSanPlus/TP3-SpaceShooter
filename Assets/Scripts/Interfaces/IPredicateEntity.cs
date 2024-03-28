namespace Interfaces
{
    public interface IPredicateEntity
    {
        public Entities.BaseEntity[] GetEntities();
        public void SetEntities(Entities.BaseEntity[] entities);
    }
}