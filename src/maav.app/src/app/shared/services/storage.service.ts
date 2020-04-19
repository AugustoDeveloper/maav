import { Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root'
  })
export class StorageService {

    /**
     * Saves the item in local storage.
     * @param  {string} key
     * @param  {string|object} value
     * @returns string
     */
    public setItem(key: string, value: string | object) {
      try {
        const stringValue = JSON.stringify(value);
        localStorage.setItem(key, stringValue);
      } catch (error) { }
    }
  
    /**
     * Gets the item from local storage.
     * @param  {string} key
     */
    public getItem<T>(key: string): T {
      try {
        const stringValue = localStorage.getItem(key);
        const value: T = JSON.parse(stringValue);
  
        return value;
      } catch (error) { }
  
      return null;
    }

    /**
     * Gets the item from local storage.
     * @param  {string} key
     */
    public getItemWithDefaultValue<T>(key: string, defaultValue: T): T {
      try {
        const stringValue = localStorage.getItem(key);
        const value: T = JSON.parse(stringValue);
  
        return  value ?? defaultValue;
      } catch (error) { }
  
      return defaultValue;
    }
  
    /**
     * Removes the item from local storage.
     * @param  {string} key
     */
    public removeItem(key: string) {
      localStorage.removeItem(key);
    }
  
  }