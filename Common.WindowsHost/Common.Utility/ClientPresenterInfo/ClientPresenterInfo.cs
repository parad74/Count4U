using System;

namespace Common.Utility
{
	public interface IClientPresenterInfo
    {
        string Name { get; set; }
        string Title { get; set; }
        Type UserControlType { get; set; }
        bool IsDefault { get; set; }
        string Description { get; set; }
    }

	public class ClientPresenterInfo : IClientPresenterInfo
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public Type UserControlType { get; set; }
        public bool IsDefault { get; set; }
        public string Description { get; set; }
    }
}