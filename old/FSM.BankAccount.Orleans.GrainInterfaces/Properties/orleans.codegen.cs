#if !EXCLUDE_CODEGEN
#pragma warning disable 162
#pragma warning disable 219
#pragma warning disable 414
#pragma warning disable 649
#pragma warning disable 693
#pragma warning disable 1591
#pragma warning disable 1998
[assembly: global::System.CodeDom.Compiler.GeneratedCodeAttribute("Orleans-CodeGenerator", "1.2.0.0")]
[assembly: global::Orleans.CodeGeneration.OrleansCodeGenerationTargetAttribute("FSM.BankAccount.Orleans.GrainInterfaces, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")]
namespace FSM.BankAccount.Orleans
{
    using global::Orleans.Async;
    using global::Orleans;

    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Orleans-CodeGenerator", "1.2.0.0"), global::System.SerializableAttribute, global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute, global::Orleans.CodeGeneration.GrainReferenceAttribute(typeof (global::FSM.BankAccount.Orleans.IBankAccount))]
    internal class OrleansCodeGenBankAccountReference : global::Orleans.Runtime.GrainReference, global::FSM.BankAccount.Orleans.IBankAccount
    {
        protected @OrleansCodeGenBankAccountReference(global::Orleans.Runtime.GrainReference @other): base (@other)
        {
        }

        protected @OrleansCodeGenBankAccountReference(global::System.Runtime.Serialization.SerializationInfo @info, global::System.Runtime.Serialization.StreamingContext @context): base (@info, @context)
        {
        }

        protected override global::System.Int32 InterfaceId
        {
            get
            {
                return -542091787;
            }
        }

        public override global::System.String InterfaceName
        {
            get
            {
                return "global::FSM.BankAccount.Orleans.IBankAccount";
            }
        }

        public override global::System.Boolean @IsCompatible(global::System.Int32 @interfaceId)
        {
            return @interfaceId == -542091787 || @interfaceId == 1450857564;
        }

        protected override global::System.String @GetMethodName(global::System.Int32 @interfaceId, global::System.Int32 @methodId)
        {
            switch (@interfaceId)
            {
                case -542091787:
                    switch (@methodId)
                    {
                        case -894876866:
                            return "Deposit";
                        case -1427812547:
                            return "Withdrawal";
                        case 110006974:
                            return "Close";
                        case 815223017:
                            return "ProcessMessage";
                        default:
                            throw new global::System.NotImplementedException("interfaceId=" + -542091787 + ",methodId=" + @methodId);
                    }

                case 1450857564:
                    switch (@methodId)
                    {
                        case 815223017:
                            return "ProcessMessage";
                        default:
                            throw new global::System.NotImplementedException("interfaceId=" + 1450857564 + ",methodId=" + @methodId);
                    }

                default:
                    throw new global::System.NotImplementedException("interfaceId=" + @interfaceId);
            }
        }

        public global::System.Threading.Tasks.Task<global::FSM.BankAccount.Orleans.BankAccountData> @Deposit(global::FSM.BankAccount.Domain.Amount @amount)
        {
            return base.@InvokeMethodAsync<global::FSM.BankAccount.Orleans.BankAccountData>(-894876866, new global::System.Object[]{@amount});
        }

        public global::System.Threading.Tasks.Task<global::FSM.BankAccount.Orleans.BankAccountData> @Withdrawal(global::FSM.BankAccount.Domain.Amount @amount)
        {
            return base.@InvokeMethodAsync<global::FSM.BankAccount.Orleans.BankAccountData>(-1427812547, new global::System.Object[]{@amount});
        }

        public global::System.Threading.Tasks.Task<global::FSM.BankAccount.Orleans.BankAccountData> @Close()
        {
            return base.@InvokeMethodAsync<global::FSM.BankAccount.Orleans.BankAccountData>(110006974, null);
        }

        public global::System.Threading.Tasks.Task<global::FSM.BankAccount.Orleans.BankAccountData> @ProcessMessage(global::FSM.BankAccount.Orleans.BankAccountMessage @message)
        {
            return base.@InvokeMethodAsync<global::FSM.BankAccount.Orleans.BankAccountData>(815223017, new global::System.Object[]{@message});
        }
    }

    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Orleans-CodeGenerator", "1.2.0.0"), global::Orleans.CodeGeneration.MethodInvokerAttribute("global::FSM.BankAccount.Orleans.IBankAccount", -542091787, typeof (global::FSM.BankAccount.Orleans.IBankAccount)), global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
    internal class OrleansCodeGenBankAccountMethodInvoker : global::Orleans.CodeGeneration.IGrainMethodInvoker
    {
        public global::System.Threading.Tasks.Task<global::System.Object> @Invoke(global::Orleans.Runtime.IAddressable @grain, global::Orleans.CodeGeneration.InvokeMethodRequest @request)
        {
            global::System.Int32 interfaceId = @request.@InterfaceId;
            global::System.Int32 methodId = @request.@MethodId;
            global::System.Object[] arguments = @request.@Arguments;
            try
            {
                if (@grain == null)
                    throw new global::System.ArgumentNullException("grain");
                switch (interfaceId)
                {
                    case -542091787:
                        switch (methodId)
                        {
                            case -894876866:
                                return ((global::FSM.BankAccount.Orleans.IBankAccount)@grain).@Deposit((global::FSM.BankAccount.Domain.Amount)arguments[0]).@Box();
                            case -1427812547:
                                return ((global::FSM.BankAccount.Orleans.IBankAccount)@grain).@Withdrawal((global::FSM.BankAccount.Domain.Amount)arguments[0]).@Box();
                            case 110006974:
                                return ((global::FSM.BankAccount.Orleans.IBankAccount)@grain).@Close().@Box();
                            case 815223017:
                                return ((global::FSM.BankAccount.Orleans.IBankAccount)@grain).@ProcessMessage((global::FSM.BankAccount.Orleans.BankAccountMessage)arguments[0]).@Box();
                            default:
                                throw new global::System.NotImplementedException("interfaceId=" + -542091787 + ",methodId=" + methodId);
                        }

                    case 1450857564:
                        switch (methodId)
                        {
                            case 815223017:
                                return ((global::FSM.BankAccount.Orleans.IBankAccount)@grain).@ProcessMessage((global::FSM.BankAccount.Orleans.BankAccountMessage)arguments[0]).@Box();
                            default:
                                throw new global::System.NotImplementedException("interfaceId=" + 1450857564 + ",methodId=" + methodId);
                        }

                    default:
                        throw new global::System.NotImplementedException("interfaceId=" + interfaceId);
                }
            }
            catch (global::System.Exception exception)
            {
                return global::Orleans.Async.TaskUtility.@Faulted(exception);
            }
        }

        public global::System.Int32 InterfaceId
        {
            get
            {
                return -542091787;
            }
        }
    }

    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Orleans-CodeGenerator", "1.2.0.0"), global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute, global::Orleans.CodeGeneration.SerializerAttribute(typeof (global::FSM.BankAccount.Domain.Amount)), global::Orleans.CodeGeneration.RegisterSerializerAttribute]
    internal class OrleansCodeGenFSM_BankAccount_Domain_AmountSerializer
    {
        private static readonly global::System.Reflection.FieldInfo field0 = typeof (global::FSM.BankAccount.Domain.Amount).@GetField("value", (System.@Reflection.@BindingFlags.@Instance | System.@Reflection.@BindingFlags.@NonPublic | System.@Reflection.@BindingFlags.@Public));
        private static readonly global::System.Func<global::FSM.BankAccount.Domain.Amount, global::System.Decimal> getField0 = (global::System.Func<global::FSM.BankAccount.Domain.Amount, global::System.Decimal>)global::Orleans.Serialization.SerializationManager.@GetGetter(field0);
        private static readonly global::System.Action<global::FSM.BankAccount.Domain.Amount, global::System.Decimal> setField0 = (global::System.Action<global::FSM.BankAccount.Domain.Amount, global::System.Decimal>)global::Orleans.Serialization.SerializationManager.@GetReferenceSetter(field0);
        [global::Orleans.CodeGeneration.CopierMethodAttribute]
        public static global::System.Object DeepCopier(global::System.Object original)
        {
            global::FSM.BankAccount.Domain.Amount input = ((global::FSM.BankAccount.Domain.Amount)original);
            global::FSM.BankAccount.Domain.Amount result = (global::FSM.BankAccount.Domain.Amount)global::System.Runtime.Serialization.FormatterServices.@GetUninitializedObject(typeof (global::FSM.BankAccount.Domain.Amount));
            setField0(result, getField0(input));
            global::Orleans.@Serialization.@SerializationContext.@Current.@RecordObject(original, result);
            return result;
        }

        [global::Orleans.CodeGeneration.SerializerMethodAttribute]
        public static void Serializer(global::System.Object untypedInput, global::Orleans.Serialization.BinaryTokenStreamWriter stream, global::System.Type expected)
        {
            global::FSM.BankAccount.Domain.Amount input = (global::FSM.BankAccount.Domain.Amount)untypedInput;
            global::Orleans.Serialization.SerializationManager.@SerializeInner(getField0(input), stream, typeof (global::System.Decimal));
        }

        [global::Orleans.CodeGeneration.DeserializerMethodAttribute]
        public static global::System.Object Deserializer(global::System.Type expected, global::Orleans.Serialization.BinaryTokenStreamReader stream)
        {
            global::FSM.BankAccount.Domain.Amount result = (global::FSM.BankAccount.Domain.Amount)global::System.Runtime.Serialization.FormatterServices.@GetUninitializedObject(typeof (global::FSM.BankAccount.Domain.Amount));
            global::Orleans.@Serialization.@DeserializationContext.@Current.@RecordObject(result);
            setField0(result, (global::System.Decimal)global::Orleans.Serialization.SerializationManager.@DeserializeInner(typeof (global::System.Decimal), stream));
            return (global::FSM.BankAccount.Domain.Amount)result;
        }

        public static void Register()
        {
            global::Orleans.Serialization.SerializationManager.@Register(typeof (global::FSM.BankAccount.Domain.Amount), DeepCopier, Serializer, Deserializer);
        }

        static OrleansCodeGenFSM_BankAccount_Domain_AmountSerializer()
        {
            Register();
        }
    }
}
#pragma warning restore 162
#pragma warning restore 219
#pragma warning restore 414
#pragma warning restore 649
#pragma warning restore 693
#pragma warning restore 1591
#pragma warning restore 1998
#endif
