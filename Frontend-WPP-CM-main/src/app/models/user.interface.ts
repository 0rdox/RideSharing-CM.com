import { Id } from './id.type';

export enum Role {
  Admin = 'ADMIN',
  User = 'USER',
}

export interface IUser {
  id: Id;
  name: string;
  emailAddress: string;
  employeeNr: string;
  hasLicense: boolean;
  role: Role;
}

export type ICreateUser = Pick<
  IUser,
  | 'name'
  | 'emailAddress'
  | 'employeeNr'
  | 'hasLicense'
  | 'role'
>;