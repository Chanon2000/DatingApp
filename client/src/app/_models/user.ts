export interface User {
  username: string;
  token: string;
}

let data: number | string = "42"; // ใส่ pipe แล้วบอกว่า data เป็น string ได้

data = "10";

// สร้าง interface เพื่อให้มันสมำ่เสมอ
interface Car {
  color: string;
  model: string;
  // topSpeed: number; // ถ้าไม่ใส่ ? มันจะหมายความว่า ทุก property นั้น requrid
  topSpeed?: number;
}

// const car1 = {
//   color: 3,
//   // color: 3, // ถ้าไม่มี interface แล้วใส่ color เป็น number มันก็ไม่มีแจ้งเตือน error
//   model: 'BMW'
// }

const car1: Car = {
  color: 'blue',
  // color: 3, // ถ้าไม่มี interface แล้วใส่ color เป็น number มันก็ไม่มีแจ้งเตือน error
  model: 'BMW'
}

const car2: Car = {
  color: 'red',
  model: 'Mercedes',
  topSpeed: 100
}

const multiply = (x: number, y: number): void => {
  x * y;
}