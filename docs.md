# Ncodi Programming Language Documentation

## Table of Contents

1. [Getting Started](##getting-started)
   - [Installation](###installation)
   - [Hello World](###hello-world)
2. [Variables](###variables)
3. [Types](##types)
   - [Int](###int)
   - [Decimal](###decimal)
   - [String](###string)
   - [Bool](###bool)
   - [Type Declaration](###type-declaration)
4. [Operators](##operators)
   - [Arithmetic](###arithmetic)
   - [Comparison](###comparison)
   - [Logical](###logical)
5. [Conditionals](##conditionals)
   - [Normal](###normal)
   - [Nested](###nested)
6. [Loops](##loops)
   - [men ... ila loop](###men--ila-loop)
   - [madem loop](###madem-loop)
   - [dir ... madem loop](###dir--madem-loop)
7. [Functions](##functions)
   - [Calling functions](###calling-functions)
   - [Built-in functions](###built-in-functions)
   - [Defining functions](###defining-functions)

## Getting Started

### Installation

#### Windows Installation

1. Download the file from [here](https://github.com/azizamari/Ncodi/blob/master/src/Ncodi.Web/setup/ncodi.exe?)
2. Update your path:
   - From the Start search bar, enter 'env' and select "Edit environment variables for your account"
   - Under User variables, check if there is an entry called Path
   - If the entry exists, append the full path to desiredNcodiFolder\ using ; as a separator from existing values
   - If the entry doesn't exist, create a new user variable named Path with the full path to desiredNcodiFolder\ as its value

#### Linux Installation

Coming soon...

### Hello World

To print something to the console, use the `ekteb()` function:

```ncodi
ekteb("Hello, Tunisia!")
```

## Variables
Variables store data for later use. Think of them as containers with a name and a value.
```ncodi
var x = "Hello again"
ekteb(x)
```
Constant variables (const) cannot be changed after declaration:
```ncodi
const x = "No Name"
x = "Ncodi"  // This will result in an error
```
## Types

Ncodi supports several data types:
### Int

Integers are whole numbers, positive or negative:
```ncodi
var num1 = 458
var num2 = 15
ekteb(num1 + num2)
```

### Decimal

Decimals are numbers with a floating point:
```ncodi
var dec = 4.25
ekteb(dec)
```

### String
Strings are textual data enclosed in quotation marks:

```ncodi
var s = "Code in your own words"
var x = "51"  // This is a string, not an integer
ekteb(s)
```

### Bool
Boolean values can be either s7i7 (true) or ghalet (false):

```ncodi
var res = s7i7
var nope = ghalet
ekteb(res)
```

### Type Declaration
You can specify the type of a variable when declaring it:

```ncodi
var num:int = 1564
var dec:decimal = 1.5
var really:bool = ghalet
var esmi:string = "Aziz Amari"
```


## Operators

### Arithmetic

Addition: +
Subtraction: -
Multiplication: *
Division: /
Euclidean Division: //
Modulo: %
Exponentiation: **

### Comparison

Greater than: >
Greater than or equal to: >=
Less than or equal to: <=
Equal to: ==
Not equal to: !=

### Logical

AND: &&
OR: ||
NOT: !

## Conditionals
### Normal
```ncodi
var x:int = int(a9ra())
kan x % 2 == 0 {
    ekteb(string(x) + " is an even number")
} wela {
    ekteb(string(x) + " is an odd number")
}
```
### Nested
```ncodi 
var language = a9ra()
kan language == "ncodi" {
    ekteb("Great Language")
} wela kan language == "python" {
    ekteb("Mehh !")
} wela {
    ekteb("I don't know this language yet: " + language)
}
```
### Loops
men ... ila loop
```ncodi 
men num = 0 ila 10 {
    ekteb(num)
}
```
### madem loop

```ncodi var lang = a9ra()
madem (lang != "ncodi" && lang != "Ncodi") {
    ekteb("Wrong, think again")
    lang = a9ra()
}
ekteb("YES")
```
### dir ... madem loop
```ncodi 
var lang = "ncodi"
dir {
    ekteb(lang)
} madem lang != "ncodi"
```

## Functions
### Calling functions
```ncodi 
ekteb("output")
ekteb(len("aziz"))
```

### Built-in functions

ekteb(): Prints to the screen
a9ra(): Prompts for user input
sqrt(): Calculates square root
random(): Generates a random number
ord(): Converts a character to its ASCII code
chr(): Converts an ASCII code to a character

### Defining functions

```ncodi 
asne3 hey() {
    ekteb("Hello from ncodi servers")
}

asne3 greet(name:string, times:int) {
    men i = 0 ila times {
        ekteb("Hello " + name + " " + string(i))
    }
}

asne3 pow(n:int, p:int):int {
    raje3 n ** p
}

// Calling the functions
hey()
greet("Aziz Amari", 4)
var x = pow(2, 8)
ekteb(x)
```
