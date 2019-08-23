using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace MySql.Data.MySqlClient.Properties
{
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0"), DebuggerNonUserCode, CompilerGenerated]
	public class Resources
	{
		private static ResourceManager resourceMan;

		private static CultureInfo resourceCulture;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static ResourceManager ResourceManager
		{
			get
			{
				if (object.ReferenceEquals(Resources.resourceMan, null))
				{
					ResourceManager resourceManager = new ResourceManager("MySql.Data.MySqlClient.Properties.Resources", typeof(Resources).Assembly);
					Resources.resourceMan = resourceManager;
				}
				return Resources.resourceMan;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static CultureInfo Culture
		{
			get
			{
				return Resources.resourceCulture;
			}
			set
			{
				Resources.resourceCulture = value;
			}
		}

		public static string AdapterIsNull
		{
			get
			{
				return Resources.ResourceManager.GetString("AdapterIsNull", Resources.resourceCulture);
			}
		}

		public static string AdapterSelectIsNull
		{
			get
			{
				return Resources.ResourceManager.GetString("AdapterSelectIsNull", Resources.resourceCulture);
			}
		}

		public static string AttemptToAccessBeforeRead
		{
			get
			{
				return Resources.ResourceManager.GetString("AttemptToAccessBeforeRead", Resources.resourceCulture);
			}
		}

		public static string AuthenticationFailed
		{
			get
			{
				return Resources.ResourceManager.GetString("AuthenticationFailed", Resources.resourceCulture);
			}
		}

		public static string AuthenticationMethodNotSupported
		{
			get
			{
				return Resources.ResourceManager.GetString("AuthenticationMethodNotSupported", Resources.resourceCulture);
			}
		}

		public static string BadVersionFormat
		{
			get
			{
				return Resources.ResourceManager.GetString("BadVersionFormat", Resources.resourceCulture);
			}
		}

		public static string BufferCannotBeNull
		{
			get
			{
				return Resources.ResourceManager.GetString("BufferCannotBeNull", Resources.resourceCulture);
			}
		}

		public static string BufferNotLargeEnough
		{
			get
			{
				return Resources.ResourceManager.GetString("BufferNotLargeEnough", Resources.resourceCulture);
			}
		}

		public static string CancelNeeds50
		{
			get
			{
				return Resources.ResourceManager.GetString("CancelNeeds50", Resources.resourceCulture);
			}
		}

		public static string CancelNotSupported
		{
			get
			{
				return Resources.ResourceManager.GetString("CancelNotSupported", Resources.resourceCulture);
			}
		}

		public static string CanNotDeriveParametersForTextCommands
		{
			get
			{
				return Resources.ResourceManager.GetString("CanNotDeriveParametersForTextCommands", Resources.resourceCulture);
			}
		}

		public static string CBMultiTableNotSupported
		{
			get
			{
				return Resources.ResourceManager.GetString("CBMultiTableNotSupported", Resources.resourceCulture);
			}
		}

		public static string CBNoKeyColumn
		{
			get
			{
				return Resources.ResourceManager.GetString("CBNoKeyColumn", Resources.resourceCulture);
			}
		}

		public static string ChaosNotSupported
		{
			get
			{
				return Resources.ResourceManager.GetString("ChaosNotSupported", Resources.resourceCulture);
			}
		}

		public static string CommandTextNotInitialized
		{
			get
			{
				return Resources.ResourceManager.GetString("CommandTextNotInitialized", Resources.resourceCulture);
			}
		}

		public static string ConnectionAlreadyOpen
		{
			get
			{
				return Resources.ResourceManager.GetString("ConnectionAlreadyOpen", Resources.resourceCulture);
			}
		}

		public static string ConnectionBroken
		{
			get
			{
				return Resources.ResourceManager.GetString("ConnectionBroken", Resources.resourceCulture);
			}
		}

		public static string ConnectionMustBeOpen
		{
			get
			{
				return Resources.ResourceManager.GetString("ConnectionMustBeOpen", Resources.resourceCulture);
			}
		}

		public static string ConnectionNotOpen
		{
			get
			{
				return Resources.ResourceManager.GetString("ConnectionNotOpen", Resources.resourceCulture);
			}
		}

		public static string ConnectionNotSet
		{
			get
			{
				return Resources.ResourceManager.GetString("ConnectionNotSet", Resources.resourceCulture);
			}
		}

		public static string CouldNotFindColumnName
		{
			get
			{
				return Resources.ResourceManager.GetString("CouldNotFindColumnName", Resources.resourceCulture);
			}
		}

		public static string CountCannotBeNegative
		{
			get
			{
				return Resources.ResourceManager.GetString("CountCannotBeNegative", Resources.resourceCulture);
			}
		}

		public static string CSNoSetLength
		{
			get
			{
				return Resources.ResourceManager.GetString("CSNoSetLength", Resources.resourceCulture);
			}
		}

		public static string DataNotInSupportedFormat
		{
			get
			{
				return Resources.ResourceManager.GetString("DataNotInSupportedFormat", Resources.resourceCulture);
			}
		}

		public static string DataReaderOpen
		{
			get
			{
				return Resources.ResourceManager.GetString("DataReaderOpen", Resources.resourceCulture);
			}
		}

		public static string DefaultEncodingNotFound
		{
			get
			{
				return Resources.ResourceManager.GetString("DefaultEncodingNotFound", Resources.resourceCulture);
			}
		}

		public static string DistributedTxnNotSupported
		{
			get
			{
				return Resources.ResourceManager.GetString("DistributedTxnNotSupported", Resources.resourceCulture);
			}
		}

		public static string ErrorCreatingSocket
		{
			get
			{
				return Resources.ResourceManager.GetString("ErrorCreatingSocket", Resources.resourceCulture);
			}
		}

		public static string FatalErrorDuringExecute
		{
			get
			{
				return Resources.ResourceManager.GetString("FatalErrorDuringExecute", Resources.resourceCulture);
			}
		}

		public static string FatalErrorDuringRead
		{
			get
			{
				return Resources.ResourceManager.GetString("FatalErrorDuringRead", Resources.resourceCulture);
			}
		}

		public static string FatalErrorReadingResult
		{
			get
			{
				return Resources.ResourceManager.GetString("FatalErrorReadingResult", Resources.resourceCulture);
			}
		}

		public static string FileBasedCertificateNotSupported
		{
			get
			{
				return Resources.ResourceManager.GetString("FileBasedCertificateNotSupported", Resources.resourceCulture);
			}
		}

		public static string FromAndLengthTooBig
		{
			get
			{
				return Resources.ResourceManager.GetString("FromAndLengthTooBig", Resources.resourceCulture);
			}
		}

		public static string FromIndexMustBeValid
		{
			get
			{
				return Resources.ResourceManager.GetString("FromIndexMustBeValid", Resources.resourceCulture);
			}
		}

		public static string GetHostEntryFailed
		{
			get
			{
				return Resources.ResourceManager.GetString("GetHostEntryFailed", Resources.resourceCulture);
			}
		}

		public static string HardProcQuery
		{
			get
			{
				return Resources.ResourceManager.GetString("HardProcQuery", Resources.resourceCulture);
			}
		}

		public static string ImproperValueFormat
		{
			get
			{
				return Resources.ResourceManager.GetString("ImproperValueFormat", Resources.resourceCulture);
			}
		}

		public static string IncorrectTransmission
		{
			get
			{
				return Resources.ResourceManager.GetString("IncorrectTransmission", Resources.resourceCulture);
			}
		}

		public static string IndexAndLengthTooBig
		{
			get
			{
				return Resources.ResourceManager.GetString("IndexAndLengthTooBig", Resources.resourceCulture);
			}
		}

		public static string IndexMustBeValid
		{
			get
			{
				return Resources.ResourceManager.GetString("IndexMustBeValid", Resources.resourceCulture);
			}
		}

		public static string InvalidColumnOrdinal
		{
			get
			{
				return Resources.ResourceManager.GetString("InvalidColumnOrdinal", Resources.resourceCulture);
			}
		}

		public static string InvalidConnectionStringValue
		{
			get
			{
				return Resources.ResourceManager.GetString("InvalidConnectionStringValue", Resources.resourceCulture);
			}
		}

		public static string InvalidProcName
		{
			get
			{
				return Resources.ResourceManager.GetString("InvalidProcName", Resources.resourceCulture);
			}
		}

		public static string InvalidValueForBoolean
		{
			get
			{
				return Resources.ResourceManager.GetString("InvalidValueForBoolean", Resources.resourceCulture);
			}
		}

		public static string KeywordNoNull
		{
			get
			{
				return Resources.ResourceManager.GetString("KeywordNoNull", Resources.resourceCulture);
			}
		}

		public static string KeywordNotSupported
		{
			get
			{
				return Resources.ResourceManager.GetString("KeywordNotSupported", Resources.resourceCulture);
			}
		}

		public static string keywords
		{
			get
			{
				return Resources.ResourceManager.GetString("keywords", Resources.resourceCulture);
			}
		}

		public static string MixedParameterNamingNotAllowed
		{
			get
			{
				return Resources.ResourceManager.GetString("MixedParameterNamingNotAllowed", Resources.resourceCulture);
			}
		}

		public static string MoreThanOneOPRow
		{
			get
			{
				return Resources.ResourceManager.GetString("MoreThanOneOPRow", Resources.resourceCulture);
			}
		}

		public static string MultipleConnectionsInTransactionNotSupported
		{
			get
			{
				return Resources.ResourceManager.GetString("MultipleConnectionsInTransactionNotSupported", Resources.resourceCulture);
			}
		}

		public static string NamedPipeNoSeek
		{
			get
			{
				return Resources.ResourceManager.GetString("NamedPipeNoSeek", Resources.resourceCulture);
			}
		}

		public static string NamedPipeNoSetLength
		{
			get
			{
				return Resources.ResourceManager.GetString("NamedPipeNoSetLength", Resources.resourceCulture);
			}
		}

		public static string NewValueShouldBeMySqlParameter
		{
			get
			{
				return Resources.ResourceManager.GetString("NewValueShouldBeMySqlParameter", Resources.resourceCulture);
			}
		}

		public static string NextResultIsClosed
		{
			get
			{
				return Resources.ResourceManager.GetString("NextResultIsClosed", Resources.resourceCulture);
			}
		}

		public static string NoBodiesAndTypeNotSet
		{
			get
			{
				return Resources.ResourceManager.GetString("NoBodiesAndTypeNotSet", Resources.resourceCulture);
			}
		}

		public static string NoNestedTransactions
		{
			get
			{
				return Resources.ResourceManager.GetString("NoNestedTransactions", Resources.resourceCulture);
			}
		}

		public static string NoServerSSLSupport
		{
			get
			{
				return Resources.ResourceManager.GetString("NoServerSSLSupport", Resources.resourceCulture);
			}
		}

		public static string NoUnixSocketsOnWindows
		{
			get
			{
				return Resources.ResourceManager.GetString("NoUnixSocketsOnWindows", Resources.resourceCulture);
			}
		}

		public static string NoWindowsIdentity
		{
			get
			{
				return Resources.ResourceManager.GetString("NoWindowsIdentity", Resources.resourceCulture);
			}
		}

		public static string ObjectDisposed
		{
			get
			{
				return Resources.ResourceManager.GetString("ObjectDisposed", Resources.resourceCulture);
			}
		}

		public static string OffsetCannotBeNegative
		{
			get
			{
				return Resources.ResourceManager.GetString("OffsetCannotBeNegative", Resources.resourceCulture);
			}
		}

		public static string OffsetMustBeValid
		{
			get
			{
				return Resources.ResourceManager.GetString("OffsetMustBeValid", Resources.resourceCulture);
			}
		}

		public static string OldPasswordsNotSupported
		{
			get
			{
				return Resources.ResourceManager.GetString("OldPasswordsNotSupported", Resources.resourceCulture);
			}
		}

		public static string ParameterAlreadyDefined
		{
			get
			{
				return Resources.ResourceManager.GetString("ParameterAlreadyDefined", Resources.resourceCulture);
			}
		}

		public static string ParameterCannotBeNegative
		{
			get
			{
				return Resources.ResourceManager.GetString("ParameterCannotBeNegative", Resources.resourceCulture);
			}
		}

		public static string ParameterCannotBeNull
		{
			get
			{
				return Resources.ResourceManager.GetString("ParameterCannotBeNull", Resources.resourceCulture);
			}
		}

		public static string ParameterIndexNotFound
		{
			get
			{
				return Resources.ResourceManager.GetString("ParameterIndexNotFound", Resources.resourceCulture);
			}
		}

		public static string ParameterIsInvalid
		{
			get
			{
				return Resources.ResourceManager.GetString("ParameterIsInvalid", Resources.resourceCulture);
			}
		}

		public static string ParameterMustBeDefined
		{
			get
			{
				return Resources.ResourceManager.GetString("ParameterMustBeDefined", Resources.resourceCulture);
			}
		}

		public static string ParameterNotFoundDuringPrepare
		{
			get
			{
				return Resources.ResourceManager.GetString("ParameterNotFoundDuringPrepare", Resources.resourceCulture);
			}
		}

		public static string PasswordMustHaveLegalChars
		{
			get
			{
				return Resources.ResourceManager.GetString("PasswordMustHaveLegalChars", Resources.resourceCulture);
			}
		}

		public static string PerfMonCategoryHelp
		{
			get
			{
				return Resources.ResourceManager.GetString("PerfMonCategoryHelp", Resources.resourceCulture);
			}
		}

		public static string PerfMonCategoryName
		{
			get
			{
				return Resources.ResourceManager.GetString("PerfMonCategoryName", Resources.resourceCulture);
			}
		}

		public static string PerfMonHardProcHelp
		{
			get
			{
				return Resources.ResourceManager.GetString("PerfMonHardProcHelp", Resources.resourceCulture);
			}
		}

		public static string PerfMonHardProcName
		{
			get
			{
				return Resources.ResourceManager.GetString("PerfMonHardProcName", Resources.resourceCulture);
			}
		}

		public static string PerfMonSoftProcHelp
		{
			get
			{
				return Resources.ResourceManager.GetString("PerfMonSoftProcHelp", Resources.resourceCulture);
			}
		}

		public static string PerfMonSoftProcName
		{
			get
			{
				return Resources.ResourceManager.GetString("PerfMonSoftProcName", Resources.resourceCulture);
			}
		}

		public static string ProcAndFuncSameName
		{
			get
			{
				return Resources.ResourceManager.GetString("ProcAndFuncSameName", Resources.resourceCulture);
			}
		}

		public static string QueryTooLarge
		{
			get
			{
				return Resources.ResourceManager.GetString("QueryTooLarge", Resources.resourceCulture);
			}
		}

		public static string ReadFromStreamFailed
		{
			get
			{
				return Resources.ResourceManager.GetString("ReadFromStreamFailed", Resources.resourceCulture);
			}
		}

		public static string ReadingPriorColumnUsingSeqAccess
		{
			get
			{
				return Resources.ResourceManager.GetString("ReadingPriorColumnUsingSeqAccess", Resources.resourceCulture);
			}
		}

		public static string ReplicatedConnectionsAllowOnlyReadonlyStatements
		{
			get
			{
				return Resources.ResourceManager.GetString("ReplicatedConnectionsAllowOnlyReadonlyStatements", Resources.resourceCulture);
			}
		}

		public static string Replication_ConnectionAttemptFailed
		{
			get
			{
				return Resources.ResourceManager.GetString("Replication_ConnectionAttemptFailed", Resources.resourceCulture);
			}
		}

		public static string Replication_NoAvailableServer
		{
			get
			{
				return Resources.ResourceManager.GetString("Replication_NoAvailableServer", Resources.resourceCulture);
			}
		}

		public static string ReplicationGroupNotFound
		{
			get
			{
				return Resources.ResourceManager.GetString("ReplicationGroupNotFound", Resources.resourceCulture);
			}
		}

		public static string ReplicationServerNotFound
		{
			get
			{
				return Resources.ResourceManager.GetString("ReplicationServerNotFound", Resources.resourceCulture);
			}
		}

		public static string RoutineNotFound
		{
			get
			{
				return Resources.ResourceManager.GetString("RoutineNotFound", Resources.resourceCulture);
			}
		}

		public static string RoutineRequiresReturnParameter
		{
			get
			{
				return Resources.ResourceManager.GetString("RoutineRequiresReturnParameter", Resources.resourceCulture);
			}
		}

		public static string ServerTooOld
		{
			get
			{
				return Resources.ResourceManager.GetString("ServerTooOld", Resources.resourceCulture);
			}
		}

		public static string SnapshotNotSupported
		{
			get
			{
				return Resources.ResourceManager.GetString("SnapshotNotSupported", Resources.resourceCulture);
			}
		}

		public static string SocketNoSeek
		{
			get
			{
				return Resources.ResourceManager.GetString("SocketNoSeek", Resources.resourceCulture);
			}
		}

		public static string SoftProcQuery
		{
			get
			{
				return Resources.ResourceManager.GetString("SoftProcQuery", Resources.resourceCulture);
			}
		}

		public static string SPNotSupported
		{
			get
			{
				return Resources.ResourceManager.GetString("SPNotSupported", Resources.resourceCulture);
			}
		}

		public static string StreamAlreadyClosed
		{
			get
			{
				return Resources.ResourceManager.GetString("StreamAlreadyClosed", Resources.resourceCulture);
			}
		}

		public static string StreamNoRead
		{
			get
			{
				return Resources.ResourceManager.GetString("StreamNoRead", Resources.resourceCulture);
			}
		}

		public static string StreamNoWrite
		{
			get
			{
				return Resources.ResourceManager.GetString("StreamNoWrite", Resources.resourceCulture);
			}
		}

		public static string Timeout
		{
			get
			{
				return Resources.ResourceManager.GetString("Timeout", Resources.resourceCulture);
			}
		}

		public static string TimeoutGettingConnection
		{
			get
			{
				return Resources.ResourceManager.GetString("TimeoutGettingConnection", Resources.resourceCulture);
			}
		}

		public static string TraceCloseConnection
		{
			get
			{
				return Resources.ResourceManager.GetString("TraceCloseConnection", Resources.resourceCulture);
			}
		}

		public static string TraceErrorMoreThanMaxValueConnections
		{
			get
			{
				return Resources.ResourceManager.GetString("TraceErrorMoreThanMaxValueConnections", Resources.resourceCulture);
			}
		}

		public static string TraceFetchError
		{
			get
			{
				return Resources.ResourceManager.GetString("TraceFetchError", Resources.resourceCulture);
			}
		}

		public static string TraceOpenConnection
		{
			get
			{
				return Resources.ResourceManager.GetString("TraceOpenConnection", Resources.resourceCulture);
			}
		}

		public static string TraceOpenResultError
		{
			get
			{
				return Resources.ResourceManager.GetString("TraceOpenResultError", Resources.resourceCulture);
			}
		}

		public static string TraceQueryDone
		{
			get
			{
				return Resources.ResourceManager.GetString("TraceQueryDone", Resources.resourceCulture);
			}
		}

		public static string TraceQueryNormalized
		{
			get
			{
				return Resources.ResourceManager.GetString("TraceQueryNormalized", Resources.resourceCulture);
			}
		}

		public static string TraceQueryOpened
		{
			get
			{
				return Resources.ResourceManager.GetString("TraceQueryOpened", Resources.resourceCulture);
			}
		}

		public static string TraceResult
		{
			get
			{
				return Resources.ResourceManager.GetString("TraceResult", Resources.resourceCulture);
			}
		}

		public static string TraceResultClosed
		{
			get
			{
				return Resources.ResourceManager.GetString("TraceResultClosed", Resources.resourceCulture);
			}
		}

		public static string TraceSetDatabase
		{
			get
			{
				return Resources.ResourceManager.GetString("TraceSetDatabase", Resources.resourceCulture);
			}
		}

		public static string TraceStatementClosed
		{
			get
			{
				return Resources.ResourceManager.GetString("TraceStatementClosed", Resources.resourceCulture);
			}
		}

		public static string TraceStatementExecuted
		{
			get
			{
				return Resources.ResourceManager.GetString("TraceStatementExecuted", Resources.resourceCulture);
			}
		}

		public static string TraceStatementPrepared
		{
			get
			{
				return Resources.ResourceManager.GetString("TraceStatementPrepared", Resources.resourceCulture);
			}
		}

		public static string TraceUAWarningBadIndex
		{
			get
			{
				return Resources.ResourceManager.GetString("TraceUAWarningBadIndex", Resources.resourceCulture);
			}
		}

		public static string TraceUAWarningFieldConversion
		{
			get
			{
				return Resources.ResourceManager.GetString("TraceUAWarningFieldConversion", Resources.resourceCulture);
			}
		}

		public static string TraceUAWarningNoIndex
		{
			get
			{
				return Resources.ResourceManager.GetString("TraceUAWarningNoIndex", Resources.resourceCulture);
			}
		}

		public static string TraceUAWarningSkippedColumns
		{
			get
			{
				return Resources.ResourceManager.GetString("TraceUAWarningSkippedColumns", Resources.resourceCulture);
			}
		}

		public static string TraceUAWarningSkippedRows
		{
			get
			{
				return Resources.ResourceManager.GetString("TraceUAWarningSkippedRows", Resources.resourceCulture);
			}
		}

		public static string TraceWarning
		{
			get
			{
				return Resources.ResourceManager.GetString("TraceWarning", Resources.resourceCulture);
			}
		}

		public static string TypeIsNotCommandInterceptor
		{
			get
			{
				return Resources.ResourceManager.GetString("TypeIsNotCommandInterceptor", Resources.resourceCulture);
			}
		}

		public static string TypeIsNotExceptionInterceptor
		{
			get
			{
				return Resources.ResourceManager.GetString("TypeIsNotExceptionInterceptor", Resources.resourceCulture);
			}
		}

		public static string UnableToConnectToHost
		{
			get
			{
				return Resources.ResourceManager.GetString("UnableToConnectToHost", Resources.resourceCulture);
			}
		}

		public static string UnableToCreateAuthPlugin
		{
			get
			{
				return Resources.ResourceManager.GetString("UnableToCreateAuthPlugin", Resources.resourceCulture);
			}
		}

		public static string UnableToDeriveParameters
		{
			get
			{
				return Resources.ResourceManager.GetString("UnableToDeriveParameters", Resources.resourceCulture);
			}
		}

		public static string UnableToEnableQueryAnalysis
		{
			get
			{
				return Resources.ResourceManager.GetString("UnableToEnableQueryAnalysis", Resources.resourceCulture);
			}
		}

		public static string UnableToEnumerateUDF
		{
			get
			{
				return Resources.ResourceManager.GetString("UnableToEnumerateUDF", Resources.resourceCulture);
			}
		}

		public static string UnableToExecuteSP
		{
			get
			{
				return Resources.ResourceManager.GetString("UnableToExecuteSP", Resources.resourceCulture);
			}
		}

		public static string UnableToParseFK
		{
			get
			{
				return Resources.ResourceManager.GetString("UnableToParseFK", Resources.resourceCulture);
			}
		}

		public static string UnableToRetrieveParameters
		{
			get
			{
				return Resources.ResourceManager.GetString("UnableToRetrieveParameters", Resources.resourceCulture);
			}
		}

		public static string UnableToStartSecondAsyncOp
		{
			get
			{
				return Resources.ResourceManager.GetString("UnableToStartSecondAsyncOp", Resources.resourceCulture);
			}
		}

		public static string UnixSocketsNotSupported
		{
			get
			{
				return Resources.ResourceManager.GetString("UnixSocketsNotSupported", Resources.resourceCulture);
			}
		}

		public static string UnknownAuthenticationMethod
		{
			get
			{
				return Resources.ResourceManager.GetString("UnknownAuthenticationMethod", Resources.resourceCulture);
			}
		}

		public static string UnknownConnectionProtocol
		{
			get
			{
				return Resources.ResourceManager.GetString("UnknownConnectionProtocol", Resources.resourceCulture);
			}
		}

		public static string ValueNotCorrectType
		{
			get
			{
				return Resources.ResourceManager.GetString("ValueNotCorrectType", Resources.resourceCulture);
			}
		}

		public static string ValueNotSupportedForGuid
		{
			get
			{
				return Resources.ResourceManager.GetString("ValueNotSupportedForGuid", Resources.resourceCulture);
			}
		}

		public static string WinAuthNotSupportOnPlatform
		{
			get
			{
				return Resources.ResourceManager.GetString("WinAuthNotSupportOnPlatform", Resources.resourceCulture);
			}
		}

		public static string WriteToStreamFailed
		{
			get
			{
				return Resources.ResourceManager.GetString("WriteToStreamFailed", Resources.resourceCulture);
			}
		}

		public static string WrongParameterName
		{
			get
			{
				return Resources.ResourceManager.GetString("WrongParameterName", Resources.resourceCulture);
			}
		}

		internal Resources()
		{
		}
	}
}
