﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.17929
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace MobileWx.Bll.FileTransServiceReference {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="FileTransServiceReference.IFileServer")]
    public interface IFileServer {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFileServer/SyncFile", ReplyAction="http://tempuri.org/IFileServer/SyncFileResponse")]
        bool SyncFile(string filePath);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFileServer/CreateFile", ReplyAction="http://tempuri.org/IFileServer/CreateFileResponse")]
        bool CreateFile(string filePath, string fileContent);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFileServer/CreateBinaryFile", ReplyAction="http://tempuri.org/IFileServer/CreateBinaryFileResponse")]
        bool CreateBinaryFile(string filePath, byte[] binaryContent);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFileServer/FileCopy", ReplyAction="http://tempuri.org/IFileServer/FileCopyResponse")]
        bool FileCopy(string filePath, string fileContent);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFileServer/BinaryFileCopy", ReplyAction="http://tempuri.org/IFileServer/BinaryFileCopyResponse")]
        bool BinaryFileCopy(string filePath, byte[] binaryContent);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IFileServerChannel : MobileWx.Bll.FileTransServiceReference.IFileServer, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class FileServerClient : System.ServiceModel.ClientBase<MobileWx.Bll.FileTransServiceReference.IFileServer>, MobileWx.Bll.FileTransServiceReference.IFileServer {
        
        public FileServerClient() {
        }
        
        public FileServerClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public FileServerClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public FileServerClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public FileServerClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public bool SyncFile(string filePath) {
            return base.Channel.SyncFile(filePath);
        }
        
        public bool CreateFile(string filePath, string fileContent) {
            return base.Channel.CreateFile(filePath, fileContent);
        }
        
        public bool CreateBinaryFile(string filePath, byte[] binaryContent) {
            return base.Channel.CreateBinaryFile(filePath, binaryContent);
        }
        
        public bool FileCopy(string filePath, string fileContent) {
            return base.Channel.FileCopy(filePath, fileContent);
        }
        
        public bool BinaryFileCopy(string filePath, byte[] binaryContent) {
            return base.Channel.BinaryFileCopy(filePath, binaryContent);
        }
    }
}
