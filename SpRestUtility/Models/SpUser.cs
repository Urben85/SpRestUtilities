namespace SP_REST_UTILITY
{
    public class SpUser
    {
        public int Id { get; set; }
        public bool IsHiddenInUi { get; set; }
        public string LoginName { get; set; }
        public string Title { get; set; }
        public SpUserPrincipalType PrincipalType { get; set; }
        public string Email { get; set; }
        public bool IsSiteAdmin { get; set; }
        public string UserId { get; set; }
    }
    public enum SpUserPrincipalType
    {
        None = 0,
        User = 1,
        DistributionList = 2,
        SecurityGroup = 4,
        SharepointGroup = 8,
        All = 15
    }
}