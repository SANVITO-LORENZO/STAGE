//DUE DIVERSE TIPOLOGIE DI DICHIARAZIONE
//let firstName = "Dylan";
//let firstName: string = "Dylan";

//TIPO ANY E NON ANY
// let u = true;
// u = "string"; // Error: Type 'string' is not assignable to type 'boolean'.
// Math.round(u); // Error: Argument of type 'boolean' is not assignable to parameter of type 'number'.

// let v: any = true;
// v = "string"; // no error as it can be "any" type
// Math.round(v); // no error as it can be "any" type

//ARREY
// const names: string[] = [];
// names.push("Dylan"); 

//ARREY CON READONLY
// const names: readonly string[] = ["Dylan"];
// names.push("Jack"); // Error: Property 'push' does not exist on type 'readonly string[]'.

//ARREY PUSH
// const numbers = [1, 2, 3]; // inferred to type number[]
// numbers.push(4); // no error
// // comment line below out to see the successful assignment
// numbers.push("2"); // Error: Argument of type 'string' is not assignable to parameter of type 'number'.
// let head: number = numbers[0]; // no error

//ARREY MULTI TIPO
// // define our tuple
// let ourTuple: [number, boolean, string];
// // initialize correctly
// ourTuple = [5, false, 'Coding God was here'];

//OGGETTO
// const car: { type: string, model: string, year: number } = {
//     type: "Toyota",
//     model: "Corolla",
//     year: 2009
//   };

//ENUMS
// enum CardinalDirections {
//     North = 'North',
//     East = "East",
//     South = "South",
//     West = "West"
//   };
//   // logs "North"
//   console.log(CardinalDirections.North);
//   // logs "West"
//   console.log(CardinalDirections.West);

//OGGETTO CRATO IN MANIERA DIVERSA
// type CarYear = number
// type CarType = string
// type CarModel = string
// type Car = {
//   year: CarYear,
//   type: CarType,
//   model: CarModel
// }
// const carYear: CarYear = 2001
// const carType: CarType = "Toyota"
// const carModel: CarModel = "Corolla"
// const car: Car = {
//   year: carYear,
//   type: carType,
//   model: carModel
// };

//INTERFACCE
// interface Rectangle {
//     height: number,
//     width: number
//   }
  
//   interface ColoredRectangle extends Rectangle {
//     color: string
//   }
  
//   const coloredRectangle: ColoredRectangle = {
//     height: 20,
//     width: 10,
//     color: "red"
//   };

//FUNZIONI
// function multiply(a: number, b: number) {
//     return a * b;
//   }

//CASTING
// let x: unknown = 'hello';
// console.log((x as string).length);

//CLASSE FUNZIONI E COSTRUTTORE
// class Person {
//     private name: string;
  
//     public constructor(name: string) {
//       this.name = name;
//     }
  
//     public getName(): string {
//       return this.name;
//     }
//   }
  
//   const person = new Person("Jane");
//   console.log(person.getName());