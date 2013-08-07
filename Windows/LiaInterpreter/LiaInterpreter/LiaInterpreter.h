// LiaInterpreter.h

#pragma once


#ifdef _WIN32
#include <msclr\marshal_cppstd.h>
using namespace System;
using namespace System::Runtime::InteropServices;
using namespace msclr::interop;
#endif




#define LIA_CODE_USER_FOR_INTERPRETER
//#define LIA_CODE_USE_FOR_COMPILER

// If defined, this keeps a note of all calls and where from in memory. This is slower, but good for debugging
#define TINYJS_CALL_STACK

#ifdef _WIN32
#ifdef _DEBUG
#define _CRTDBG_MAP_ALLOC
#include <stdlib.h>
#include <crtdbg.h>



#endif
#endif
#include <string>
#include <vector>



#ifndef TRACE
#define TRACE printf
#endif // TRACE

namespace Lia {
	const int TINYJS_LOOP_MAX_ITERATIONS = 8192;

	enum LEX_TYPES {
		LEX_EOF = 0,
		LEX_ID = 256,
		LEX_INT,
		LEX_FLOAT,
		LEX_STR,

		LEX_EQUAL,
		LEX_TYPEEQUAL,
		LEX_NEQUAL,
		LEX_NTYPEEQUAL,
		LEX_LEQUAL,
		LEX_LSHIFT,
		LEX_LSHIFTEQUAL,
		LEX_GEQUAL,
		LEX_RSHIFT,
		LEX_RSHIFTUNSIGNED,
		LEX_RSHIFTEQUAL,
		LEX_PLUSEQUAL,
		LEX_MINUSEQUAL,
		LEX_PLUSPLUS,
		LEX_MINUSMINUS,
		LEX_ANDEQUAL,
		LEX_ANDAND,
		LEX_OREQUAL,
		LEX_OROR,
		LEX_XOREQUAL,
		// reserved words
#define LEX_R_LIST_START LEX_R_IF
		LEX_R_IF,
		LEX_R_ELSE,
		LEX_R_DO,
		LEX_R_WHILE,
		LEX_R_FOR,
		LEX_R_BREAK,
		LEX_R_CONTINUE,
		LEX_R_FUNCTION,
		LEX_R_RETURN,
		LEX_R_VAR,
		LEX_R_TRUE,
		LEX_R_FALSE,
		LEX_R_NULL,
		LEX_R_UNDEFINED,
		LEX_R_NEW,
#ifdef LIA_CODE_USE_FOR_COMPILER
		LEX_R_IMPORT,
#endif
		LEX_R_LIST_END /* always the last entry */
	};

	enum SCRIPTVAR_FLAGS {
		SCRIPTVAR_UNDEFINED   = 0,
		SCRIPTVAR_FUNCTION    = 1,
		SCRIPTVAR_OBJECT      = 2,
		SCRIPTVAR_ARRAY       = 4,
		SCRIPTVAR_DOUBLE      = 8,  // floating point double
		SCRIPTVAR_INTEGER     = 16, // integer number
		SCRIPTVAR_STRING      = 32, // string
		SCRIPTVAR_NULL        = 64, // it seems null is its own data type

		SCRIPTVAR_NATIVE      = 128, // to specify this is a native function
		SCRIPTVAR_NUMERICMASK = SCRIPTVAR_NULL |
		SCRIPTVAR_DOUBLE |
		SCRIPTVAR_INTEGER,
		SCRIPTVAR_VARTYPEMASK = SCRIPTVAR_DOUBLE |
		SCRIPTVAR_INTEGER |
		SCRIPTVAR_STRING |
		SCRIPTVAR_FUNCTION |
		SCRIPTVAR_OBJECT |
		SCRIPTVAR_ARRAY |
		SCRIPTVAR_NULL,

	};

#define TINYJS_RETURN_VAR "return"
#define TINYJS_PROTOTYPE_CLASS "class"
#define TINYJS_TEMP_NAME ""
#define TINYJS_BLANK_DATA ""

	/// convert the given string into a quoted string suitable for javascript
	std::string getJSString(const std::string &str);

	public class CScriptException {
	public:
		std::string text;
		CScriptException(const std::string &exceptionText);
	};

	public class CScriptLex
	{
	public:
		CScriptLex(const std::string &input);
		CScriptLex(CScriptLex *owner, int startChar, int endChar);
		~CScriptLex(void);

		char currCh, nextCh;
		int tk; ///< The type of the token that we have
		int tokenStart; ///< Position in the data at the beginning of the token we have here
		int tokenEnd; ///< Position in the data at the last character of the token we have here
		int tokenLastEnd; ///< Position in the data at the last character of the last token
		std::string tkStr; ///< Data contained in the token we have here

		void match(int expected_tk); ///< Lexical match wotsit
		static std::string getTokenStr(int token); ///< Get the string representation of the given token
		void reset(); ///< Reset this lex so we can start again

		std::string getSubString(int pos); ///< Return a sub-string from the given position up until right now
		CScriptLex *getSubLex(int lastPosition); ///< Return a sub-lexer from the given position up until right now

		std::string getPosition(int pos=-1); ///< Return a string representing the position in lines and columns of the character pos given

		//protected:
		/* When we go into a loop, we use getSubLex to get a lexer for just the sub-part of the
		relevant string. This doesn't re-allocate and copy the string, but instead copies
		the data pointer and sets dataOwned to false, and dataStart/dataEnd to the relevant things. */
		char *data; ///< Data string to get tokens from
		int dataStart, dataEnd; ///< Start and end position in data string
		bool dataOwned; ///< Do we own this data string?

		int dataPos; ///< Position in data (we CAN go past the end of the string here)

		void getNextCh();
		void getNextToken(); ///< Get the text token from our text string
	};

	class CScriptVar;

	typedef void (*JSCallback)(CScriptVar *var, void * userdata);

	public class CScriptVarLink
	{
	public:
		std::string name;
		CScriptVarLink *nextSibling;
		CScriptVarLink *prevSibling;
		CScriptVar *var;
		bool owned;

		CScriptVarLink(CScriptVar *var, const std::string &name = TINYJS_TEMP_NAME);
		CScriptVarLink(const CScriptVarLink &link); ///< Copy constructor
		~CScriptVarLink();
		void replaceWith(CScriptVar *newVar); ///< Replace the Variable pointed to
		void replaceWith(CScriptVarLink *newVar); ///< Replace the Variable pointed to (just dereferences)
		int getIntName(); ///< Get the name as an integer (for arrays)
		void setIntName(int n); ///< Set the name as an integer (for arrays)
	};

	/// Variable class (containing a doubly-linked list of children)
	public class CScriptVar
	{
	public:
		CScriptVar(); ///< Create undefined
		CScriptVar(const std::string &varData, int varFlags); ///< User defined
		CScriptVar(const std::string &str); ///< Create a string
		CScriptVar(double varData);
		CScriptVar(int val);
		~CScriptVar(void);

		CScriptVar *getReturnVar(); ///< If this is a function, get the result value (for use by native functions)
		void setReturnVar(CScriptVar *var); ///< Set the result value. Use this when setting complex return data as it avoids a deepCopy()
		CScriptVar *getParameter(const std::string &name); ///< If this is a function, get the parameter with the given name (for use by native functions)

		CScriptVarLink *findChild(const std::string &childName); ///< Tries to find a child with the given name, may return 0
		CScriptVarLink *findChildOrCreate(const std::string &childName, int varFlags=SCRIPTVAR_UNDEFINED); ///< Tries to find a child with the given name, or will create it with the given flags
		CScriptVarLink *findChildOrCreateByPath(const std::string &path); ///< Tries to find a child with the given path (separated by dots)
		CScriptVarLink *addChild(const std::string &childName, CScriptVar *child=NULL);
		CScriptVarLink *addChildNoDup(const std::string &childName, CScriptVar *child=NULL); ///< add a child overwriting any with the same name
		void removeChild(CScriptVar *child);
		void removeLink(CScriptVarLink *link); ///< Remove a specific link (this is faster than finding via a child)
		void removeAllChildren();
		CScriptVar *getArrayIndex(int idx); ///< The the value at an array index
		void setArrayIndex(int idx, CScriptVar *value); ///< Set the value at an array index
		int getArrayLength(); ///< If this is an array, return the number of items in it (else 0)
		int getChildren(); ///< Get the number of children

		int getInt();
		bool getBool() { return getInt() != 0; }
		double getDouble();
		const std::string &getString();
		std::string getParsableString(); ///< get Data as a parsable javascript string
		void setInt(int num);
		void setDouble(double val);
		void setString(const std::string &str);
		void setUndefined();
		void setArray();
		bool equals(CScriptVar *v);

		bool isInt() { return (flags&SCRIPTVAR_INTEGER)!=0; }
		bool isDouble() { return (flags&SCRIPTVAR_DOUBLE)!=0; }
		bool isString() { return (flags&SCRIPTVAR_STRING)!=0; }
		bool isNumeric() { return (flags&SCRIPTVAR_NUMERICMASK)!=0; }
		bool isFunction() { return (flags&SCRIPTVAR_FUNCTION)!=0; }
		bool isObject() { return (flags&SCRIPTVAR_OBJECT)!=0; }
		bool isArray() { return (flags&SCRIPTVAR_ARRAY)!=0; }
		bool isNative() { return (flags&SCRIPTVAR_NATIVE)!=0; }
		bool isUndefined() { return (flags & SCRIPTVAR_VARTYPEMASK) == SCRIPTVAR_UNDEFINED; }
		bool isNull() { return (flags & SCRIPTVAR_NULL)!=0; }
		bool isBasic() { return firstChild==0; } ///< Is this *not* an array/object/etc

		CScriptVar *mathsOp(CScriptVar *b, int op); ///< do a maths op with another script variable
		void copyValue(CScriptVar *val); ///< copy the value from the value given
		CScriptVar *deepCopy(); ///< deep copy this node and return the result

		void trace(std::string indentStr = "", const std::string &name = ""); ///< Dump out the contents of this using trace
		std::string getFlagsAsString(); ///< For debugging - just dump a string version of the flags
		void getJSON(std::ostringstream &destination, const std::string linePrefix=""); ///< Write out all the JS code needed to recreate this script variable to the stream (as JSON)
		void setCallback(JSCallback callback, void * userdata); ///< Set the callback for native functions

		CScriptVarLink *firstChild;
		CScriptVarLink *lastChild;

		/// For memory management/garbage collection
		CScriptVar *ref(); ///< Add reference to this variable
		void unref(); ///< Remove a reference, and delete this variable if required
		int getRefs(); ///< Get the number of references to this script variable
		//protected:
		int refs; ///< The number of references held to this - used for garbage collection

		std::string data; ///< The contents of this variable if it is a string
		long intData; ///< The contents of this variable if it is an int
		double doubleData; ///< The contents of this variable if it is a double
		int flags; ///< the flags determine the type of the variable - int/double/string/etc
		JSCallback jsCallback; ///< Callback for native functions
		void * jsCallbackUserData; ///< user data passed as second argument to native functions

		void init(); ///< initialisation of data members

		/** Copy the basic data and flags from the variable given, with no
		* children. Should be used internally only - by copyValue and deepCopy */
		void copySimpleData(CScriptVar *val);

		friend class CTinyJS;
	};

	static std::string TARGET;

	public class CTinyJS {
	public:
		CTinyJS();
		~CTinyJS();
		
		void execute(const std::string &code);

		/** Evaluate the given code and return a link to a javascript object,
		* useful for (dangerous) JSON parsing. If nothing to return, will return
		* 'undefined' variable type. CScriptVarLink is returned as this will
		* automatically unref the result as it goes out of scope. If you want to
		* keep it, you must use ref() and unref() */
		CScriptVarLink evaluateComplex(const std::string &code);
		/** Evaluate the given code and return a string. If nothing to return, will return
		* 'undefined' */
		std::string evaluate(const std::string &code);

		/// add a native function to be called from TinyJS
		/** example:
		\code
		void scRandInt(CScriptVar *c, void *userdata) { ... }
		tinyJS->addNative("function randInt(min, max)", scRandInt, 0);
		\endcode

		or

		\code
		void scSubstring(CScriptVar *c, void *userdata) { ... }
		tinyJS->addNative("function String.substring(lo, hi)", scSubstring, 0);
		\endcode
		*/
		void addNative(const std::string &funcDesc, JSCallback ptr, void * userdata);

		/// Get the given variable specified by a path (var1.var2.etc), or return 0
		CScriptVar *getScriptVariable(const std::string &path);
		/// Get the value of the given variable, or return 0
		const std::string *getVariable(const std::string &path);
		/// set the value of the given variable, return trur if it exists and gets set
		bool setVariable(const std::string &path, const std::string &varData);

		/// Send all variables to stdout
		void trace();

		CScriptVar *root;   /// root of symbol table
	protected:

		CScriptLex *l;             /// current lexer
		std::vector<CScriptVar*> scopes; /// stack of scopes when parsing
#ifdef TINYJS_CALL_STACK
		std::vector<std::string> call_stack; /// Names of places called so we can show when erroring
#endif

		CScriptVar *stringClass; /// Built in string class
		CScriptVar *objectClass; /// Built in object class
		CScriptVar *arrayClass; /// Built in array class

		// parsing - in order of precedence
		CScriptVarLink *functionCall(bool &execute, CScriptVarLink *function, CScriptVar *parent);
		CScriptVarLink *factor(bool &execute);
		CScriptVarLink *unary(bool &execute);
		CScriptVarLink *term(bool &execute);
		CScriptVarLink *expression(bool &execute);
		CScriptVarLink *shift(bool &execute);
		CScriptVarLink *condition(bool &execute);
		CScriptVarLink *logic(bool &execute);
		CScriptVarLink *ternary(bool &execute);
		CScriptVarLink *base(bool &execute);
		void block(bool &execute);
		void statement(bool &execute);
		// parsing utility functions
		CScriptVarLink *parseFunctionDefinition();
		void parseFunctionArguments(CScriptVar *funcVar);

		CScriptVarLink *findInScopes(const std::string &childName); ///< Finds a child, looking recursively up the scopes
		/// Look up in any parent classes of the given object
		CScriptVarLink *findInParentClasses(CScriptVar *object, const std::string &name);
	};

#ifdef LIA_CODE_USE_FOR_COMPILER

	public class LiaPreCompiler : public CTinyJS
	{
		std::string mainPath;
		std::string code;
	public:

		std::vector<std::string> * list;
		LiaPreCompiler(std::string path, std::vector<std::string> * list);
		std::string precompile(std::string code);
		void statement(bool &execute);
	};
#endif


	static std::string cs2cpString(System::String ^ s )
	{
		return marshal_as<std::string>(s);
	}
	static System::String ^ cp2csString(std::string s)
	{
		return marshal_as<System::String ^>(s);
	}



	public ref class LiaInterpreterVar
	{
		CScriptVar * var;
	public : 
		LiaInterpreterVar()
		{
			var = new CScriptVar();
		}
		LiaInterpreterVar(CScriptVar * org)
		{
			var = org;
		}
		LiaInterpreterVar(System::String ^ varData, int varFlags)
		{
			var = new CScriptVar(cs2cpString(varData),varFlags);
		}///< User defined
		LiaInterpreterVar(System::String ^ str)
		{
			var = new CScriptVar(cs2cpString(str));
		}///< Create a string
		LiaInterpreterVar(double varData)
		{
			var = new CScriptVar(varData);
		}
		LiaInterpreterVar(int val)
		{
			var = new CScriptVar(val);
		}
		~LiaInterpreterVar(void)
		{
			delete var;
		}

		LiaInterpreterVar ^ getReturnVar()
		{
			return gcnew LiaInterpreterVar(var->getReturnVar());
		}///< If this is a function, get the result value (for use by native functions)
		void setReturnVar(LiaInterpreterVar ^ ret)
		{
			var->setReturnVar(ret->var);
		}///< Set the result value. Use this when setting complex return data as it avoids a deepCopy()
		LiaInterpreterVar ^ getParameter(System::String ^ name)
		{
			return gcnew LiaInterpreterVar(var->getParameter(cs2cpString(name)));
		}///< If this is a function, get the parameter with the given name (for use by native functions)
		/*
		CScriptVarLink *findChild(System::String ^ childName)
		{

		}///< Tries to find a child with the given name, may return 0
		CScriptVarLink *findChildOrCreate(const std::string &childName, int varFlags=SCRIPTVAR_UNDEFINED)
		{

		}///< Tries to find a child with the given name, or will create it with the given flags
		CScriptVarLink *findChildOrCreateByPath(const std::string &path); ///< Tries to find a child with the given path (separated by dots)
		CScriptVarLink *addChild(const std::string &childName, CScriptVar *child=NULL);
		CScriptVarLink *addChildNoDup(const std::string &childName, CScriptVar *child=NULL); ///< add a child overwriting any with the same name
		void removeChild(CScriptVar *child);
		void removeLink(CScriptVarLink *link); ///< Remove a specific link (this is faster than finding via a child)
		void removeAllChildren();
		*/
		LiaInterpreterVar ^getArrayIndex(int idx)
		{
			return gcnew LiaInterpreterVar(var->getArrayIndex(idx));
		}///< The the value at an array index
		void setArrayIndex(int idx, LiaInterpreterVar ^value)
		{
			var->setArrayIndex(idx, value->var);
		}///< Set the value at an array index
		int getArrayLength()
		{
			return var->getArrayLength();
		}///< If this is an array, return the number of items in it (else 0)
		int getChildren()
		{
			return var->getChildren();
		}///< Get the number of children

		int getInt()
		{
			return var->getInt();
		}
		bool getBool() { return var->getInt() != 0; }
		double getDouble()
		{
			return var->getDouble();
		}
		System::String ^ getString()
		{
			return cp2csString(var->getString());
		}
		System::String ^ getParsableString()
		{
			return cp2csString(var->getParsableString());
		}///< get Data as a parsable javascript string

		void setInt(int num)
		{
			var->setInt(num);
		}
		void setDouble(double val)
		{
			var->setDouble(val);
		}
		void setString(System::String ^ str)
		{
			var->setString(cs2cpString(str));
		}
		void setUndefined()
		{
			var->setUndefined();
		}
		void setArray()
		{
			var->setArray();
		}
		bool equals(LiaInterpreterVar ^ v)
		{
			return var->equals(v->var);
		}

		bool isInt() { return (var->flags&SCRIPTVAR_INTEGER)!=0; }
		bool isDouble() { return (var->flags&SCRIPTVAR_DOUBLE)!=0; }
		bool isString() { return (var->flags&SCRIPTVAR_STRING)!=0; }
		bool isNumeric() { return (var->flags&SCRIPTVAR_NUMERICMASK)!=0; }
		bool isFunction() { return (var->flags&SCRIPTVAR_FUNCTION)!=0; }
		bool isObject() { return (var->flags&SCRIPTVAR_OBJECT)!=0; }
		bool isArray() { return (var->flags&SCRIPTVAR_ARRAY)!=0; }
		bool isNative() { return (var->flags&SCRIPTVAR_NATIVE)!=0; }
		bool isUndefined() { return (var->flags & SCRIPTVAR_VARTYPEMASK) == SCRIPTVAR_UNDEFINED; }
		bool isNull() { return (var->flags & SCRIPTVAR_NULL)!=0; }
		bool isBasic() { return var->firstChild==0; } ///< Is this *not* an array/object/etc
	};

	public ref class LiaInterpreter
	{
		CTinyJS * tinyJS;
	public:

		delegate void(CSCallBack)(LiaInterpreterVar ^ liaVar);

		bool setVariable(System::String ^ path, System::String ^ varData)
		{
			return tinyJS->setVariable(cs2cpString(path),cs2cpString(varData));
		}

		LiaInterpreter(System::String ^ target)
		{
			Lia::TARGET = cs2cpString(target);
			tinyJS = new CTinyJS();
		}

		~LiaInterpreter()
		{
			delete tinyJS;
		}

		System::String^ call(System::String^ code)
		{
			try
			{
				tinyJS->execute(cs2cpString(code) + "main();");
			}
			catch(CScriptException *e)
			{
				return cp2csString(e->text);
			}

			return cp2csString("END");
		}

		void addNative(System::String ^ funcDesc, CSCallBack ^ ptr);
	};

	void CSCallBackCenterFunction(CScriptVar * var, void * userdata)
	{
		LiaInterpreter::CSCallBack ^ csCall = (LiaInterpreter::CSCallBack  ^)Marshal::GetDelegateForFunctionPointer(IntPtr(userdata) ,LiaInterpreter::CSCallBack::typeid);
		csCall(gcnew LiaInterpreterVar(var));
	}
}
