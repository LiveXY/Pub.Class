using System.Reflection.Emit;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Collections;
using System.Runtime.InteropServices;

namespace Pub.Class {
    /// <summary>    
    /// MSIL操作类  
    /// </summary>
    public class ILHelper {
        #region attributes & properties
        private ILGenerator m_il;
        private List<string> m_variables = null;
        private Dictionary<string, Label> m_Labels;
        /// <summary>
        /// <para>eng - Returns the ILGenerator used in Helper .</para>
        /// <para>pt-Br - Retorna o ILGenerator usado no Helper.</para>    
        /// </summary>
        public ILGenerator IL {
            get { return m_il; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="il">
        /// <para>eng - IL to manipulate.</para>
        /// <para>pt-Br - IL para manipular.</para>    
        /// </param>
        public ILHelper(ILGenerator il) {
            m_il = il;
        }
        #endregion

        #region CreateLocalVar
        /// <summary>
        /// <para>eng - Create a local variable.</para>
        /// <para>pt-Br - Cria uma variavel local.</para> 
        /// </summary>
        /// <param name="name">
        /// <para>eng - Name to the variable.</para>
        /// <para>pt-Br - Nome da váriavel.</para> 
        /// </param>
        /// <param name="type">
        /// <para>eng - Type of variable.</para>
        /// <para>pt-Br - Tipo da varável.</para> 
        /// </param>
        /// <param name="pinned">
        /// <para>eng - Indicates if must  pin the variable in memory.</para>
        /// <para>pt-Br - Indica se deve segurar na memória a variável.</para> 
        /// </param>
        /// <returns></returns>
        public ILHelper CreateLocalVar(string name, Type type, bool pinned) {
            if (this.m_variables.IsNull())
                this.m_variables = new List<string>();
            else if (this.m_variables.Contains(name) == true)
                return this;

            this.m_variables.Add(name);
            this.m_il.DeclareLocal(type, pinned);

            return this;
        }
        #endregion

        #region DefineConstructor
        /// <summary>
        /// <para>eng - Define a constructor.</para>
        /// <para>pt-Br - Define um construtor.</para>
        /// </summary>
        /// <param name="contructor"></param>
        /// <returns></returns>
        public ILHelper DefineConstructor(ConstructorInfo contructor) {
            this.m_il.Emit(OpCodes.Call, contructor);
            return this;
        }
        #endregion

        #region InvokeMethod
        /// <summary>
        /// <para>eng - Invoke a method.</para>
        /// <para>pt-Br - Chama um método.</para>
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <returns></returns>
        public ILHelper InvokeMethod(MethodInfo methodInfo) {
            this.m_il.EmitCall(OpCodes.Call, methodInfo, null);
            return this;
        }

        /// <summary>
        /// <para>eng - Invoke a method.</para>
        /// <para>pt-Br - Chama um método.</para>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="bindFlag"></param>
        /// <returns></returns>
        public ILHelper InvokeMethod(Type type, string name, BindingFlags bindFlag) {
            var method = type.GetMethod(name, bindFlag);
            if (method.IsNull())
                throw new Exception("Cannot found method : " + name);

            return InvokeMethod(method);
        }
        /// <summary>
        /// <para>eng - Invoke a method by name and type.</para>
        /// <para>pt-Br - Chama um método pelo nome e tipo.</para>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public ILHelper InvokeMethod(Type type, string name) {
            BindingFlags bindFlag = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
            return InvokeMethod(type, name, bindFlag);
        }
        /// <summary>
        /// <para>eng - Emit a Opcode if condition is true.</para>
        /// <para>pt-Br - Emite um Opcode se a condição for verdadeira.</para>
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="opCode"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public ILHelper EmitIf(bool condition, OpCode opCode, int value) {
            if (condition)
                this.m_il.Emit(opCode, value);
            return this;
        }
        #endregion

        #region LoadString
        /// <summary>
        /// <para>eng - Load a string in Stack.</para>
        /// <para>pt-Br - Carrega uma string no Stack.</para>
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public ILHelper LoadString(string text) {
            this.m_il.Emit(OpCodes.Ldstr, text);
            return this;
        }
        #endregion

        #region Return
        /// <summary>
        /// <para>eng - Emit Return.</para>
        /// <para>pt-Br - Emite um retorno.</para>
        /// </summary>
        public ILHelper Return() {
            this.m_il.Emit(OpCodes.Ret);
            return this;
        }
        #endregion

        #region Try
        /// <summary>
        /// <para>eng - Begin try block (like "try {").</para>
        /// <para>pt-Br - Inicia o bloco do try ("try {").</para>
        /// </summary>
        public ILHelper BeginTry() {
            this.m_il.BeginExceptionBlock();
            return this;
        }
        /// <summary>
        /// <para>eng - Begin try block (like "try {").</para>
        /// <para>pt-Br - Inicia o bloco do try ("try {").</para>
        /// <param name="label">
        /// <para>eng - Returns Label created.</para>
        /// <para>pt-Br - Retorna o Label criado.</para>
        /// </param>
        /// </summary>
        public ILHelper BeginTry(out Label label) {
            label = this.m_il.BeginExceptionBlock();
            return this;
        }
        /// <summary>
        /// <para>eng - End try block (like "}").</para>
        /// <para>pt-Br - Finaliza o bloco do try ("}").</para>
        /// </summary>
        public ILHelper EndTry() {
            this.m_il.EndExceptionBlock();
            return this;
        }
        /// <summary>
        /// <para>eng - Begin catch block (like "catch {").</para>
        /// <para>pt-Br - Inicia o bloco do catch ("catch {").</para>
        /// </summary>
        public ILHelper BeginCatch() {
            this.m_il.BeginCatchBlock(typeof(Exception));
            return this;
        }
        /// <summary>
        /// <para>eng - Begin catch block (like "catch {").</para>
        /// <para>pt-Br - Inicia o bloco do catch ("catch {").</para>
        /// <param name="type">
        /// <para>eng - Exception type.</para>
        /// <para>pt-Br - Tipo do exception.</para>
        /// </param>
        /// </summary>
        public ILHelper BeginCatch(Type type) {
            this.m_il.BeginCatchBlock(type);
            return this;
        }

        /// <summary>
        /// <para>eng - Begin finally block (like "finally {").</para>
        /// <para>pt-Br - Inicia o bloco do finally ("finally {").</para>
        /// </summary>
        /// <returns></returns>
        public ILHelper BeginFinally() {
            this.m_il.BeginFinallyBlock();
            return this;
        }
        /// <summary>
        /// <para>eng - Rethrow exception.</para>
        /// <para>pt-Br - Executa o throw com a exception corrente.</para>
        /// </summary>
        public ILHelper ReThrow() {
            this.m_il.Emit(OpCodes.Rethrow);
            return this;
        }
        /// <summary>
        /// <para>eng - Throw exception.</para>
        /// <para>pt-Br - Executa o throw.</para>
        /// </summary>
        /// <param name="excpType">
        /// <para>eng - Exception type.</para>
        /// <para>pt-Br - Tipo do exception.</para>
        /// </param>
        /// <param name="errorMessage">
        /// <para>eng - Message error.</para>
        /// <para>pt-Br - mensagem de erro.</para>
        /// </param>
        /// <returns></returns>
        public ILHelper Throw(Type excpType, string errorMessage) {
            this.m_il.Emit(OpCodes.Ldstr, errorMessage);
            this.NewObj(excpType, typeof(string));
            this.m_il.Emit(OpCodes.Throw);
            return this;
        }
        /// <summary>
        /// <para>eng - Create a new object calling the class constructor.</para>
        /// <para>pt-Br - Cria um novo objeto chamando o costrutor da classe.</para>
        /// </summary>
        /// <param name="type">
        /// <para>eng - Type of object.</para>
        /// <para>pt-Br - Tipo do objeto.</para>
        /// </param>
        /// <param name="parameters">
        /// <para>eng - Contructor parameters.</para>
        /// <para>pt-Br - Parametros do construtor.</para>
        /// </param>
        /// <returns></returns>
        public ILHelper NewObj(Type type, params Type[] parameters) {
            this.m_il.Emit(OpCodes.Newobj, type.GetConstructor(parameters));
            return this;
        }
        #endregion

        #region Label
        /// <summary>
        /// <para>eng - Mark a label in code (like ":Label"). Create a new label if don't exist.</para>
        /// <para>pt-Br - Gera um label no código (":meuLabel"). Cria um novo label se não existe.</para>
        /// </summary>
        /// <param name="name">
        /// <para>eng - Label name.</para>
        /// <para>pt-Br - Nome do label.</para>
        /// </param>
        /// <returns></returns>
        public ILHelper MarkLabel(string name) {
            if (m_Labels.IsNull() || !m_Labels.ContainsKey(name))
                CreateLabel(name);

            Label label = m_Labels[name];

            this.m_il.MarkLabel(label);
            return this;
        }
        /// <summary>
        /// <para>eng - Define a label in code (like ":Label").</para>
        /// <para>pt-Br - Cria um label no código (":meuLabel").</para>
        /// </summary>
        /// <param name="name">
        /// <para>eng - Label name.</para>
        /// <para>pt-Br - Nome do label.</para>
        /// </param>
        /// <returns></returns>
        public ILHelper CreateLabel(string name) {
            if (m_Labels.IsNull())
                m_Labels = new Dictionary<string, Label>();
            else if (m_Labels.ContainsKey(name))
                throw new InvalidOperationException("Label already exist");

            Label lb = this.m_il.DefineLabel();
            m_Labels.Add(name, lb);
            return this;
        }
        #endregion

        #region GotoIfNotNull
        /// <summary>
        /// <para>eng - Goto label if the stack is true or not null.</para>
        /// <para>pt-Br - Excuta um goto para o label se for true ou se não for nulo.</para>
        /// </summary>
        /// <param name="name">
        /// <para>eng - Label name.</para>
        /// <para>pt-Br - Nome do label.</para>
        /// </param>
        /// <returns></returns>
        public ILHelper GotoIfNotNullOrTrue(string name) {
            if (this.m_Labels.IsNull() || !this.m_Labels.ContainsKey(name))
                this.CreateLabel(name);

            this.m_il.Emit(OpCodes.Brtrue, m_Labels[name]);
            return this;
        }
        /// <summary>
        /// <para>eng - Goto label if the stack is false or null.</para>
        /// <para>pt-Br - Excuta um goto para o label se for false ou se for nulo.</para>
        /// </summary>
        /// <param name="name">
        /// <para>eng - Label name.</para>
        /// <para>pt-Br - Nome do label.</para>
        /// </param>
        /// <returns></returns>
        public ILHelper GotoIfNullOrFalse(string name) {
            if (this.m_Labels.IsNull() || !this.m_Labels.ContainsKey(name))
                this.CreateLabel(name);

            this.m_il.Emit(OpCodes.Brfalse, m_Labels[name]);
            return this;
        }
        #endregion

        #region Goto
        /// <summary>
        /// <para>eng - Goto label .</para>
        /// <para>pt-Br - Excuta um goto para o label .</para>
        /// </summary>
        /// <param name="name">
        /// <para>eng - Label name.</para>
        /// <para>pt-Br - Nome do label.</para>
        /// </param>
        /// <returns></returns>
        public ILHelper Goto(string name) {
            if (this.m_Labels.IsNull() || !this.m_Labels.ContainsKey(name))
                this.CreateLabel(name);

            this.m_il.Emit(OpCodes.Br_S, m_Labels[name]);
            return this;
        }
        #endregion

        /// <summary>
        /// <para>eng - Load a Field in Stack.</para>
        /// <para>pt-Br - Carrega um Field no Stack.</para>
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public ILHelper LoadField(Type objectType, string fieldName) {
            BindingFlags bindFlag = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
            var field = objectType.GetField(fieldName, bindFlag);
            this.m_il.Emit(OpCodes.Ldfld, field);
            return this;
        }
        /// <summary>
        /// <para>eng - Copies a value to memory .</para>
        /// <para>pt-Br - Copia um valor para a memória.</para>
        /// </summary>
        /// <param name="value">
        /// <para>eng - Object value .</para>
        /// <para>pt-Br - Valor de um objeto.</para>
        /// </param>
        /// <returns></returns>
        public ILHelper SetObj(object value) {
            this.m_il.Emit(OpCodes.Stobj, value.GetType());
            return this;
        }
        /// <summary>
        /// <para>eng - Load a bool value in Stack.</para>
        /// <para>pt-Br - Carrega um valor booleano no Stack.</para>
        /// </summary>
        /// <param name="value">
        /// <para>eng - bool value .</para>
        /// <para>pt-Br - Valor bolleano.</para>
        /// </param>
        /// <returns></returns>
        public ILHelper LoadBool(bool value) {
            this.m_il.Emit(value ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
            this.Box(typeof(bool));
            return this;
        }
        /// <summary>
        /// <para>eng - Load a object value in Stack.</para>
        /// <para>pt-Br - Carrega um objeto no Stack.</para>
        /// </summary>
        /// <param name="value">
        /// <para>eng - Object value .</para>
        /// <para>pt-Br - Valor do objeto.</para>
        /// </param>
        /// <returns></returns>
        public ILHelper LoadObject(object value) {
            if (value is string)
                this.LoadString((string)value);
            else if (value is bool)
                this.LoadBool((bool)value);
            else if (value is int) {
                this.LoadInt((int)value);
                //this.Box(typeof(object));
            } else
                this.LoadObj(value);

            return this;
        }
        /// <summary>
        /// <para>eng - Load a object value in Stack.</para>
        /// <para>pt-Br - Carrega um objeto no Stack.</para>
        /// </summary>
        /// <param name="value">
        /// <para>eng - Object value .</para>
        /// <para>pt-Br - Valor do objeto.</para>
        /// </param>
        /// <returns></returns>
        public ILHelper LoadObj(object value) {
            this.m_il.Emit(OpCodes.Ldobj, value.GetType());
            return this;
        }
        /// <summary>
        /// <para>eng - Returns the OpCode by type.</para>
        /// <para>pt-Br - Retorna o OpCode pelo tipo informado.</para>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private OpCode ReturnOpcodeByType(Type type) {
            var ldindOpCodes = new Dictionary<Type, OpCode>();
            ldindOpCodes[typeof(sbyte)] = OpCodes.Ldind_I1;
            ldindOpCodes[typeof(short)] = OpCodes.Ldind_I2;
            ldindOpCodes[typeof(int)] = OpCodes.Ldind_I4;
            ldindOpCodes[typeof(long)] = OpCodes.Ldind_I8;
            ldindOpCodes[typeof(byte)] = OpCodes.Ldind_U1;
            ldindOpCodes[typeof(ushort)] = OpCodes.Ldind_U2;
            ldindOpCodes[typeof(uint)] = OpCodes.Ldind_U4;
            ldindOpCodes[typeof(ulong)] = OpCodes.Ldind_I8;
            ldindOpCodes[typeof(float)] = OpCodes.Ldind_R4;
            ldindOpCodes[typeof(double)] = OpCodes.Ldind_R8;
            ldindOpCodes[typeof(char)] = OpCodes.Ldind_U2;
            ldindOpCodes[typeof(bool)] = OpCodes.Ldind_I1;

            var opCodeObject = ldindOpCodes[type];

            if (opCodeObject.IsNotNull())
                return opCodeObject;

            return OpCodes.Ldobj;

        }

        #region Box
        /// <summary>
        /// <para>eng - Converts a value type to a object reference .</para>
        /// <para>pt-Br - Converte um tipo de valor para uma referência de objeto.</para>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public ILHelper Box(Type type) {
            if (type.IsValueType)
                this.m_il.Emit(OpCodes.Box, type);
            return this;
        }
        /// <summary>
        /// <para>eng - Converts the boxed representation of type to a unboxed form.</para>
        /// <para>pt-Br - Converte um valor no box para sua forma .</para>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public ILHelper Unbox_Any(Type type) {
            if (type.IsValueType)
                this.m_il.Emit(OpCodes.Unbox_Any, type);
            return this;
        }
        #endregion

        #region SetVar
        /// <summary>
        /// <para>eng - Pops the current value to a top in the stack and store in a variable.</para>
        /// <para>pt-Br - Converte um valor no box para sua forma .</para>
        /// </summary>
        /// <param name="name">
        /// <para>eng - Variable name.</para>
        /// <para>pt-Br - Nome da variável.</para>
        /// </param>
        /// <returns></returns>
        public ILHelper SetVar(string name) {
            if (this.m_variables.IsNull())
                throw new InvalidOperationException("Don't exist any variable");

            int position = this.m_variables.IndexOf(name);

            if (position < 0)
                throw new InvalidOperationException("Don't exist variable with this name");

            return this.SetVar(position);
        }
        /// <summary>
        /// <para>eng - Pops the current value to a top in the stack and store in a variable.</para>
        /// <para>pt-Br - Converte um valor no box para sua forma .</para>
        /// </summary>
        /// <param name="position">
        /// <para>eng - Index of variables in stack.</para>
        /// <para>pt-Br - Indice da variável na memória.</para>
        /// </param>
        /// <returns></returns>
        public ILHelper SetVar(int position) {
            switch (position) {
                case 0:
                    return this.SetVar0();
                case 1:
                    return this.SetVar1();
                case 2:
                    return this.SetVar2();
                case 3:
                    return this.SetVar3();
            }
            this.m_il.Emit(OpCodes.Stloc, position);
            return this;
        }

        private ILHelper SetVar0() {
            this.m_il.Emit(OpCodes.Stloc_0);
            return this;
        }
        private ILHelper SetVar1() {
            this.m_il.Emit(OpCodes.Stloc_1);
            return this;
        }
        private ILHelper SetVar2() {
            this.m_il.Emit(OpCodes.Stloc_2);
            return this;
        }
        private ILHelper SetVar3() {
            this.m_il.Emit(OpCodes.Stloc_3);
            return this;
        }

        #endregion

        #region Load Variables
        /// <summary>
        /// <para>eng - Loads the variable from name to evaluation stack .</para>
        /// <para>pt-Br - Carrega a variavel no stack .</para>
        /// </summary>
        /// <param name="name">
        /// <para>eng - Variable name.</para>
        /// <para>pt-Br - Nome da variável.</para>
        /// </param>
        /// <returns></returns>
        public ILHelper LoadVar(string name) {
            if (this.m_variables.IsNull())
                throw new InvalidOperationException("Don't exist any variable");

            int position = this.m_variables.IndexOf(name);

            if (position < 0)
                throw new InvalidOperationException("Don't exist variable with this name");

            return this.LoadVar(position);
        }
        /// <summary>
        /// <para>eng - Loads the variable from name to evaluation stack .</para>
        /// <para>pt-Br - Carrega a variavel no stack .</para>
        /// </summary>
        /// <param name="position">
        /// <para>eng - Index of variables in stack.</para>
        /// <para>pt-Br - Indice da variável na memória.</para>
        /// </param>
        /// <returns></returns>
        public ILHelper LoadVar(int position) {
            switch (position) {
                case 0:
                    return this.LoadVar0();
                case 1:
                    return this.LoadVar1();
                case 2:
                    return this.LoadVar2();
                case 3:
                    return this.LoadVar3();
            }
            this.m_il.Emit(OpCodes.Ldloc, position);
            return this;
        }

        private ILHelper LoadVar0() {
            this.m_il.Emit(OpCodes.Ldloc_0);
            return this;
        }
        private ILHelper LoadVar1() {
            this.m_il.Emit(OpCodes.Ldloc_1);
            return this;
        }
        private ILHelper LoadVar2() {
            this.m_il.Emit(OpCodes.Ldloc_2);
            return this;
        }
        private ILHelper LoadVar3() {
            this.m_il.Emit(OpCodes.Ldloc_3);
            return this;
        }

        #endregion

        #region Load Arguments
        /// <summary>
        /// <para>eng - Loads the argument this (arg 0) onto the stack .</para>
        /// <para>pt-Br - Carrega o objeto this (arg 0) no stack .</para>
        /// </summary>
        /// <returns></returns>
        public ILHelper LoadThis() {
            return this.LoadArgument0();
        }
        /// <summary>
        /// <para>eng - Loads the argument referenced by a specified index (position) onto the stack .</para>
        /// <para>pt-Br - Carrega o parâmetro na definido pelo indice (position)  no stack .</para>
        /// </summary>
        /// <param name="position">
        /// <para>eng - Index of variables in stack.</para>
        /// <para>pt-Br - Indice da variável na memória.</para>
        /// </param>
        /// <returns></returns>
        public ILHelper LoadArgument(int position) {
            switch (position) {
                case 0:
                    return this.LoadArgument0();
                case 1:
                    return this.LoadArgument1();
                case 2:
                    return this.LoadArgument2();
                case 3:
                    return this.LoadArgument3();
            }
            this.m_il.Emit(OpCodes.Ldarg, position);
            return this;
        }

        private ILHelper LoadArgument0() {
            this.m_il.Emit(OpCodes.Ldarg_0);
            return this;
        }
        private ILHelper LoadArgument1() {
            this.m_il.Emit(OpCodes.Ldarg_1);
            return this;
        }
        private ILHelper LoadArgument2() {
            this.m_il.Emit(OpCodes.Ldarg_2);
            return this;
        }
        private ILHelper LoadArgument3() {
            this.m_il.Emit(OpCodes.Ldarg_3);
            return this;
        }

        #endregion

        #region Arrays
        /// <summary>
        /// <para>eng - Pushes an object reference to a new zero-based, one-dimensional array whose elements are of a specific type onto the evaluation stack .</para>
        /// <para>pt-Br - Cria um array com nova matriz baseada em zero, unidimensional cujos elementos são de um tipo específico na pilha de avaliação .</para>
        /// </summary>
        /// <param name="type">
        /// <para>eng - Type of array.</para>
        /// <para>pt-Br - Tipo do array.</para>
        /// </param>
        /// <returns></returns>
        public ILHelper ArrayCreate(Type type) {
            this.m_il.Emit(OpCodes.Newarr, type);
            return this;
        }
        /// <summary>
        /// <para>eng - Pushes an object reference to a new zero-based, one-dimensional array whose elements are of a specific type onto the evaluation stack .</para>
        /// <para>pt-Br - Cria um array com nova matriz baseada em zero, unidimensional cujos elementos são de um tipo específico na pilha de avaliação .</para>
        /// </summary>
        /// <param name="type">
        /// <para>eng - Type of array.</para>
        /// <para>pt-Br - Tipo do array.</para>
        /// </param>
        /// <param name="size">
        /// <para>eng - Size of array.</para>
        /// <para>pt-Br - Tamanho do array.</para>
        /// </param>
        /// <returns></returns>
        public ILHelper ArrayCreate(Type type, int size) {
            this.LoadInt(size);
            this.m_il.Emit(OpCodes.Newarr, type);
            return this;
        }
        /// <summary>
        /// <para>eng - Replaces the array element at a given index with the object ref value (type O) on the evaluation stack.</para>
        /// <para>pt-Br - Substitui o elemento do array em um determinado índice com o valor do objeto no topo na pilha.</para>
        /// </summary>
        /// <returns></returns>
        public ILHelper ArrayAdd() {
            this.m_il.Emit(OpCodes.Stelem_Ref);
            return this;
        }
        #endregion

        #region Load Int
        /// <summary>
        /// <para>eng - Pushes a supplied value of type int32 onto the evaluation stack as an int32.</para>
        /// <para>pt-Br - Coloca um valor fornecido do tipo int32 na pilha de avaliação como um int32.</para>
        /// </summary>
        /// <param name="value">
        /// <para>eng - Value to push onto the stack.</para>
        /// <para>pt-Br - Valor para carregar no stack.</para>
        /// </param>
        /// <returns></returns>
        public ILHelper LoadInt(int value) {
            //if(IntPtr.Size == 8)
            //{
            //    // 64 bit machine
            //    this.m_il.Emit(OpCodes.Ldc_I8, value);
            //}
            //else if(IntPtr.Size == 4)
            //{
            switch (value) {
                case -1:
                    return LoadIntM1();
                case 0:
                    return LoadInt0();
                case 1:
                    return LoadInt1();
                case 2:
                    return LoadInt2();
                case 3:
                    return LoadInt3();
                case 4:
                    return LoadInt4();
                case 5:
                    return LoadInt5();
                case 6:
                    return LoadInt6();
                case 7:
                    return LoadInt7();
                case 8:
                    return LoadInt8();
            }

            // 32 bit machine
            this.m_il.Emit(OpCodes.Ldc_I4, value);
            //1} 

            return this;
        }

        #region Aux

        private ILHelper LoadIntM1() {
            this.m_il.Emit(OpCodes.Ldc_I4_M1);
            return this;
        }
        private ILHelper LoadInt0() {
            this.m_il.Emit(OpCodes.Ldc_I4_0);
            return this;
        }
        private ILHelper LoadInt1() {
            this.m_il.Emit(OpCodes.Ldc_I4_1);
            return this;
        }
        private ILHelper LoadInt2() {
            this.m_il.Emit(OpCodes.Ldc_I4_2);
            return this;
        }
        private ILHelper LoadInt3() {
            this.m_il.Emit(OpCodes.Ldc_I4_3);
            return this;
        }
        private ILHelper LoadInt4() {
            this.m_il.Emit(OpCodes.Ldc_I4_4);
            return this;
        }
        private ILHelper LoadInt5() {
            this.m_il.Emit(OpCodes.Ldc_I4_5);
            return this;
        }
        private ILHelper LoadInt6() {
            this.m_il.Emit(OpCodes.Ldc_I4_6);
            return this;
        }
        private ILHelper LoadInt7() {
            this.m_il.Emit(OpCodes.Ldc_I4_7);
            return this;
        }
        private ILHelper LoadInt8() {
            this.m_il.Emit(OpCodes.Ldc_I4_8);
            return this;
        }
        #endregion

        #endregion
    }
}
