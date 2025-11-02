using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Threading.Tasks;

/*  Requerimento 1: Colocar el tipo de dato en asm dependiendo del tipo de dato en asm
    Requerimento 2: Crear las operaciones en Asignacion 
    Requerimento 3: printf
    Requerimento 4: scanf
    Requerimento 5: do
    Requerimento 6: while
*/
namespace Semantica
{
    public class Lenguaje : Sintaxis
    {
        List<Variable> variables;
        List<string> prints;
        Stack<float> s;
        int countIF, countPrint, countDo, countWhile, countFor;
        public Lenguaje()
        {
            variables = new List<Variable>();
            prints = new List<string>();
            s = new Stack<float>();
            countIF = 0;
            countPrint = 0;
            countDo = 0;
            countWhile = 0;
            countFor = 0;
        }
        public Lenguaje(string nombre) : base(nombre)
        {
            variables = new List<Variable>();
            prints = new List<string>();
            s = new Stack<float>();
            countIF = 0;
            countPrint = 0;
            countDo = 0;
            countWhile = 0;
            countFor = 0;
        }
        //Programa  -> Librerias? Variables? Main
        public void Programa()
        {
            if (getContenido() == "#")
            {
                Librerias();
            }
            if (getClasificacion() == Tipos.tipoDatos)
            {
                Variables();
            }
            asm.WriteLine("%include \"io.inc\"");
            asm.WriteLine("section .text");
            asm.WriteLine("extern io_print_string");
            asm.WriteLine("global main");
            asm.WriteLine("main:");
            Main();
            asm.WriteLine("ret");
            asm.WriteLine("section .data");
            declaraPrints();
            imprimeVariables();


        }
        private void imprimeVariables()
        {
            log.WriteLine("Variables: ");

            foreach (Variable v in variables)
            {
                log.WriteLine(v.getNombre() + " = " + v.getTipo() + " = " + v.getValor());
                asm.WriteLine(v.getNombre() + " : " + getTipoAsm("" + v.getTipo()) + " 0");
            }
        }

        private string getTipoAsm(string tipo)
        {
            switch (tipo)
            {
                case "Int":
                    return "DW";
                case "Float":
                    return "DD";
                case "Char":
                    return "DB";
                default:
                    return "DW";
            }
        }

        private void declaraPrints()
        {
            foreach (string s in prints)
            {
                asm.WriteLine(s);
            }
        }

        private void imprimeStack()
        {
            Console.WriteLine("Stack: ");
            foreach (float f in s)
            {
                Console.WriteLine(f);
            }
        }

        private bool existeVariable(string nombre)
        {
            foreach (Variable v in variables)
            {
                if (nombre == v.getNombre())
                {
                    return true;
                }
            }
            return false;
        }
        private float valorVariable(string nombre)
        {
            foreach (Variable v in variables)
            {
                if (nombre == v.getNombre())
                {
                    return v.getValor();
                }
            }
            return 0;
        }
        private void modificaValor(string nombre, float nuevoValor)
        {
            foreach (Variable v in variables)
            {
                if (nombre == v.getNombre())
                {
                    float maxNumber = float.MaxValue;
                    switch (v.getTipo())
                    {
                        case Variable.TipoDato.Char:
                            maxNumber = 256;
                            break;
                        case Variable.TipoDato.Int:
                            maxNumber = 65536;
                            break;
                    }
                    if (nuevoValor < maxNumber)
                    {
                        v.setValor(nuevoValor);
                    }
                    else
                    {
                        throw new Error(" value out of bounds");
                    }
                }
            }
        }
        //Librerias -> #include<identificador(.h)?> Librerias?
        private void Librerias()
        {
            match("#");
            match("include");
            match("<");
            match(Tipos.Identificador);
            if (getContenido() == ".")
            {
                match(".");
                match("h");
            }
            match(">");
            if (getContenido() == "#")
            {
                Librerias();
            }
        }
        //Variables -> tipoDato listaIdentificadores; Variables?
        private void Variables()
        {
            Variable.TipoDato tipo = Variable.TipoDato.Char;
            switch (getContenido())
            {
                case "int":
                    tipo = Variable.TipoDato.Int;
                    break;
                case "float":
                    tipo = Variable.TipoDato.Float;
                    break;
            }
            match(Tipos.tipoDatos);
            listaIdentificadores(tipo);
            match(";");
            if (getClasificacion() == Tipos.tipoDatos)
            {
                Variables();
            }
        }
        //listaIdentificadores -> Identificador (,listaIdentificadores)?
        private void listaIdentificadores(Variable.TipoDato tipo)
        {
            string nombre = getContenido();
            match(Tipos.Identificador);
            if (!existeVariable(nombre))
            {
                variables.Add(new Variable(nombre, tipo));
            }
            else
            {
                throw new Error("de Sintaxis : la variable " + nombre + " ya existe", log, linea);
            }
            if (getContenido() == ",")
            {
                match(",");
                listaIdentificadores(tipo);
            }
        }
        //bloqueInstrucciones -> { listaIntrucciones? }
        private void bloqueInstrucciones(bool evalua)//parametro bool evalua 
        {
            match("{");
            if (getContenido() != "}")
            {
                ListaInstrucciones(evalua);
            }
            match("}");
        }
        //ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void ListaInstrucciones(bool evalua)
        {
            Instruccion(evalua);
            if (getContenido() != "}")
            {
                ListaInstrucciones(evalua);
            }
        }
        //Instruccion -> Printf | Scanf | If | While | do while | For | Asignacion
        private void Instruccion(bool evalua)
        {
            if (getContenido() == "printf")
            {
                Printf(evalua);
            }
            else if (getContenido() == "scanf")
            {
                Scanf(evalua);
            }
            else if (getContenido() == "if")
            {
                If(evalua);
            }
            else if (getContenido() == "while")
            {
                While(evalua);
            }
            else if (getContenido() == "do")
            {
                Do(evalua);
            }
            else if (getContenido() == "for")
            {
                For(evalua);
            }
            else
            {
                Asignacion(evalua);
            }
        }
        //    Requerimiento 1: Printf -> printf(cadena(, Identificador)?);
        private void Printf(bool evalua)
        {
            match("printf");
            match("(");
            string nombreVariable = "";
            string cadenaAsm = "'" + getContenido().Trim('"').Replace("\\n", "',0Dh,'").Replace("\\t", "',09h,'") + "',0";
            string cadena = getContenido().Trim('"').Replace("\\n", "\n").Replace("\\t", "\t");
            match(Tipos.Cadena);
            if (evalua)
            {
                prints.Add("cadPrint" + (++countPrint) + " DB " + cadenaAsm);
                asm.WriteLine("MOV EAX,cadPrint" + countPrint);
                asm.WriteLine("CALL io_print_string");
                Console.Write(cadena);
            }

            if (getContenido() == ",")
            {
                match(",");

                nombreVariable = getContenido();
                match(Tipos.Identificador);
                if (!existeVariable(nombreVariable))
                {
                    throw new Error(" de Sintaxis: variable " + nombreVariable + " no declarada ", log, linea);
                }

                if (evalua)
                {
                    asm.WriteLine("MOV EAX, [" + nombreVariable + "]");
                    asm.WriteLine("PRINT_DEC 4,EAX");
                    Console.Write(valorVariable(nombreVariable));

                }

            }

            match(")");
            match(";");
        }


        //    Requerimiento 2: Scanf -> scanf(cadena,&Identificador);
        private void Scanf(bool evalua)
        {
            match("scanf");
            match("(");
            if (getContenido() != "&")
            {
                match(Tipos.Cadena);
                match(",");
            }
            match("&");
            string nombre = getContenido();
            match(Tipos.Identificador);//1
            string valor = "";
            if (evalua)
            {
                asm.WriteLine("GET_DEC 4,EAX");
                valor = "" + Console.ReadLine();
            }
            bool isNumeric = int.TryParse(valor, out _);
            if (!isNumeric)
            {
                throw new Error("de Sintaxis: Not a number");
            }
            else if (valor == "")
            {
                throw new Error("de Sintaxis: Null pointer exception");
            }
            else
            {
                if (existeVariable(nombre))
                {
                    asm.WriteLine("MOV [" + nombre + "],EAX");
                    modificaValor(nombre, float.Parse(valor));
                    match(")");
                    match(";");
                }
                else
                {
                    throw new Error(" de Sintaxis: variable no declarada ", log, linea);
                }
            }
        }

        //Asignacion -> Identificador (++ | --) | (+= | -=) Expresion | (= Expresion) ;
        private void Asignacion(bool evalua)
        {
            string nombre = getContenido();
            match(Tipos.Identificador);

            if (!existeVariable(nombre))
            {
                throw new Error(" de Sintaxis: variable " + nombre + " no declarada ", log, linea);
            }

            string operador = getContenido();
            match(getClasificacion());

            switch (operador)
            {
                case "+=":
                case "-=":
                    Expresion();
                    float valor = s.Pop();
                    float resultado = valorVariable(nombre) + (operador == "+=" ? valor : -valor);
                    if (evalua)
                    {
                        modificaValor(nombre, resultado);
                        asm.WriteLine("POP EAX");
                        asm.WriteLine("MOV EBX,[" + nombre + "]");
                        if (operador == "+=")
                        {
                            asm.WriteLine("ADD EBX,EAX");
                        }
                        if (operador == "-=")
                        {
                            asm.WriteLine("SUB EBX,EAX");
                        }
                        asm.WriteLine("MOV [" + nombre + "],EBX");
                    }
                    break;

                case "++":
                case "--":
                    resultado = valorVariable(nombre) + (operador == "++" ? 1 : -1);
                    if (evalua)
                        modificaValor(nombre, resultado);
                    if (operador == "++")
                    {
                        asm.WriteLine("MOV AX," + "[" + nombre + "]");
                        asm.WriteLine("ADD EAX,1");
                        asm.WriteLine("MOV [" + nombre + "],EAX");
                    }
                    if (operador == "--")
                    {
                        asm.WriteLine("MOV AX," + "[" + nombre + "]");
                        asm.WriteLine("SUB EAX,1");
                        asm.WriteLine("MOV [" + nombre + "],EAX");
                    }
                    break;

                case "*=":
                case "/=":
                    Expresion();
                    float nuevoValor = valorVariable(nombre);
                    nuevoValor = operador == "*=" ? nuevoValor * s.Pop() : nuevoValor / s.Pop();
                    if (evalua)
                    {
                        asm.WriteLine("MOV EDX,0");
                        asm.WriteLine("POP EBX");
                        asm.WriteLine("MOV EAX,[" + nombre + "]");
                        if (operador == "*=")
                        {
                            asm.WriteLine("MUL EBX");
                        }
                        if (operador == "/=")
                        {
                            asm.WriteLine("DIV EBX");
                        }
                        asm.WriteLine("MOV [" + nombre + "],EAX");
                        modificaValor(nombre, nuevoValor);
                    }
                    break;

                case "=":
                    Expresion();
                    if (evalua)
                        asm.WriteLine("POP EAX");
                    asm.WriteLine("MOV [" + nombre + "],EAX");
                    modificaValor(nombre, s.Pop());
                    break;

                default:
                    throw new Error("Operador de asignación no válido: " + operador, log, linea);
            }

            match(";");
        }

        //If -> if (Condicion) instruccion | bloqueInstrucciones 
        //      (else instruccion | bloqueInstrucciones)?
        private void If(bool evaluaif)
        {
            int cont = ++countIF;
            match("if");
            match("(");
            string etiqueta = "etiquetaIF" + (cont);
            bool evalua = Condicion(etiqueta) && (evaluaif);
            //Console.WriteLine(evalua);
            match(")");
            if (getContenido() == "{")
            {
                bloqueInstrucciones(evaluaif);
            }
            else
            {
                Instruccion(evalua);
            }
            asm.WriteLine("JMP finIf" + cont);
            asm.WriteLine(etiqueta + ":");
            if (getContenido() == "else")
            {
                match("else");
                if (getContenido() == "{")
                {
                    bloqueInstrucciones(evaluaif);
                }
                else
                {
                    Instruccion(!evalua && evaluaif == true);
                }
            }
            asm.WriteLine("finIf" + cont + ":");
        }
        //Condicion -> Expresion operadoRelacional Expresion
        private bool Condicion(string etiqueta)
        {
            Expresion(); // E1
            string operador = getContenido();
            match(Tipos.OperadorRelacional);
            Expresion(); // E2
            asm.WriteLine("POP EBX");
            float E2 = s.Pop();
            asm.WriteLine("POP EAX");
            float E1 = s.Pop();
            asm.WriteLine("CMP EAX,EBX");
            switch (operador)
            {
                case ">":
                    asm.WriteLine("JBE " + etiqueta);
                    return E1 > E2;
                case ">=":
                    asm.WriteLine("JB " + etiqueta);
                    return E1 >= E2;
                case "<":
                    asm.WriteLine("JAE " + etiqueta);
                    return E1 < E2;
                case "<=":
                    asm.WriteLine("JA " + etiqueta);
                    return E1 <= E2;
                case "==":
                    asm.WriteLine("JNE " + etiqueta);
                    return E1 == E2;
                default:
                    asm.WriteLine("JE " + etiqueta);
                    return E1 != E2;
            }
        }
        //While -> while(Condicion) bloqueInstrucciones | Instruccion
        private void While(bool evalua)
        {
            string etiqueta = "etiquetaWhile" + (++countWhile);
            match("while");
            match("(");
            asm.WriteLine(etiqueta + ":");
            String variable = getContenido();
            int countemp = ccount;
            int lineatemp = linea;
            bool evaluaWhile = true;
            evaluaWhile = Condicion(etiqueta + "fin") && evalua;
            match(")");
            if (getContenido() == "{")
            {
                bloqueInstrucciones(evaluaWhile);
                asm.WriteLine("JMP " + etiqueta);
            }
            else
            {
                Instruccion(evaluaWhile);
                asm.WriteLine("JMP " + etiqueta);
            }
            if (evaluaWhile)
            {
                // ccount = countemp - variable.Length;
                // linea = lineatemp;
                // archivo.DiscardBufferedData();
                // archivo.BaseStream.Seek(ccount, SeekOrigin.Begin); ;
                // nextToken();
            }
            asm.WriteLine(etiqueta + "fin :");
        }
        //Do -> do bloqueInstrucciones | Intruccion while(Condicion);
        private void Do(bool evalua)
        {
            string etiqueta = "etiquetaDo" + (++countDo);
            asm.WriteLine(etiqueta + ":");
            int countemp = ccount;
            int lineatemp = linea;
            bool evaluaDoWhile = true;

            match("do");
            if (getContenido() == "{")
            {
                bloqueInstrucciones(evalua);
            }
            else
            {
                Instruccion(evalua);
            }
            match("while");
            match("(");
            evaluaDoWhile = Condicion(etiqueta + "fin") && evalua;
            asm.WriteLine("JMP " + etiqueta);
            match(")");
            match(";");
            if (evaluaDoWhile)
            {
                // ccount = countemp - 2;
                // linea = lineatemp;
                // archivo.DiscardBufferedData();
                // archivo.BaseStream.Seek(ccount, SeekOrigin.Begin); ;
                // nextToken();
            }
            asm.WriteLine(etiqueta + "fin:");
        }
        //For -> for(Asignacion Condicion; Incremento) BloqueInstruccones | Instruccion 
        private void For(bool evalua)
        {
            match("for");
            match("(");
            int contador = ++countFor;

            Asignacion(evalua);
            string variable = "";
            string variablIncremento = getContenido();
            int countemp = ccount;
            int lineatemp = linea;
            bool evaluaFor = true;
            asm.WriteLine("for" + contador + ":");
            evaluaFor = Condicion("FinFor" + contador) && evalua;
            match(";");
            variable = getContenido();//Coreccion de incremento de variable
                                      // if(!variable.Equals(variablIncremento)){
                                      //     throw new Error("de Sintaxis diferente variable en incremento",log,linea);
                                      // }
            int inc = Incremento(evaluaFor);
            match(")");
            if (getContenido() == "{")
            {
                bloqueInstrucciones(evaluaFor);
            }
            else
            {
                Instruccion(evaluaFor);
            }
            if (evaluaFor)
            {
                modificaValor(variable, valorVariable(variable) + inc);

                //  ccount = countemp - variable.Length;
                // linea = lineatemp;
                //  archivo.DiscardBufferedData();
                //  archivo.BaseStream.Seek(ccount, SeekOrigin.Begin); ;
                //  nextToken();
                string incrementoAsm="";
                if(inc==1){
                    incrementoAsm = "INC";
                }else{
                    incrementoAsm = "DEC";
                }
                asm.WriteLine("MOV ECX,["+variable+"]");
                asm.WriteLine(incrementoAsm+" ECX");
                asm.WriteLine("MOV ["+variable+"],ECX");
            }
            asm.WriteLine("JMP for" + contador);

             asm.WriteLine("FinFor"+contador+":");
        }
        //Incremento -> Identificador ++ | --
        private int Incremento(bool evalua)
        {
            string nombre = getContenido();

            bool incremento = false;
            match(Tipos.Identificador);//1
            if (!existeVariable(nombre))
            {

            }
            if (getClasificacion() == Tipos.IncrementoTermino)
            {
                if (getContenido() == "++")
                {
                    incremento = true;
                }
                match(Tipos.IncrementoTermino);

            }
            if (incremento)
            {

                return 1;
            }
            else
            {

                return -1;
            }
            

        }
        //Main      -> void main() bloqueInstrucciones
        private void Main()
        {
            match("void");
            match("main");
            match("(");
            match(")");
            bloqueInstrucciones(true);
        }
        //Expresion -> Termino MasTermino
        private void Expresion()
        {
            Termino();
            MasTermino();
        }
        //MasTermino -> (OperadorTermino Termino)?
        private void MasTermino()
        {
            if (getClasificacion() == Tipos.OperadorTermino)
            {
                string operador = getContenido();
                match(Tipos.OperadorTermino);
                Termino();
                float N2 = s.Pop();
                asm.WriteLine("POP EBX");
                float N1 = s.Pop();
                asm.WriteLine("POP EAX");
                switch (operador)
                {
                    case "+":
                        asm.WriteLine("ADD EAX, EBX");
                        asm.WriteLine("PUSH EAX");
                        s.Push(N1 + N2);
                        break;
                    case "-":
                        asm.WriteLine("SUB EAX, EBX");
                        asm.WriteLine("PUSH EAX");
                        s.Push(N1 - N2);
                        break;
                }
            }
        }
        //Termino -> Factor PorFactor
        private void Termino()
        {
            Factor();
            PorFactor();

        }
        //PorFactor -> (OperadorFactor Factor)?
        private void PorFactor()
        {
            if (getClasificacion() == Tipos.OperadorFactor)
            {
                string operador = getContenido();
                match(Tipos.OperadorFactor);
                Factor();
                float N2 = s.Pop();
                asm.WriteLine("POP EBX");
                float N1 = s.Pop();
                asm.WriteLine("POP EAX");
                switch (operador)
                {
                    case "*":
                        asm.WriteLine("MUL EBX");
                        asm.WriteLine("PUSH EAX");
                        s.Push(N1 * N2); break;
                    case "/":
                        asm.WriteLine("MOV EDX,0");
                        asm.WriteLine("DIV EBX");
                        asm.WriteLine("PUSH EAX");
                        s.Push(N1 / N2); break;
                    case "%":
                        asm.WriteLine("MOV EDX,0");
                        asm.WriteLine("DIV EBX");
                        asm.WriteLine("PUSH EDX");
                        s.Push(N1 % N2); break;
                }

            }
        }
        //Factor -> numero | identificador | (Expresion)
        private void Factor()
        {
            if (getClasificacion() == Tipos.Numero)
            {
                asm.WriteLine("MOV EAX," + getContenido() + "");
                asm.WriteLine("PUSH EAX");
                s.Push(float.Parse(getContenido()));
                match(Tipos.Numero);
            }
            else if (getClasificacion() == Tipos.Identificador)
            {
                if (!existeVariable(getContenido()))
                {
                    throw new Error(" de Sintaxis: variable " + getContenido() + " no declarada ", log, linea);
                }

                asm.WriteLine("MOV AX,[" + getContenido() + "]");
                asm.WriteLine("PUSH EAX");
                s.Push(valorVariable(getContenido()));//2
                match(Tipos.Identificador);//1
            }
            else
            {
                match("(");
                if (getClasificacion() == Tipos.tipoDatos)
                {
                    string tipo = getContenido();
                    match(Tipos.tipoDatos);
                    match(")");
                    Expresion();
                    float pilaValue = s.Pop();
                    asm.WriteLine("POP EAX");
                    switch (tipo)
                    {
                        case "char":
                            pilaValue = pilaValue % 256;
                            break;
                        case "int":
                            pilaValue = pilaValue % 65535;
                            break;
                    }
                    asm.WriteLine("MOV EAX," + pilaValue);
                    asm.WriteLine("PUSH EAX");
                    s.Push(pilaValue);
                }
                else
                {
                    Expresion();
                    match(")");

                }


            }
        }
    }
}