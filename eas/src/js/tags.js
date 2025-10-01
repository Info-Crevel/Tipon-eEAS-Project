/* jslint esversion: 6  */

export const NotificationType = Object.freeze({
  Advice: 1,
  Confirmation: 2,
  Wait: 3,
  Lookup: 4
});

export const DataTypeId = Object.freeze({
  String: 'S',
  Integer: 'I',
  Decimal: 'F',
  Boolean: 'B',
  Date: 'D',
  Time: 'T'
});

export const FormState = Object.freeze({
  Reset: 0,
  Filled: 1
});
