import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'RoundBigNumbers' })
export class RoundBigNumbersPipe implements PipeTransform {
   
    transform(num: number) {
        // hundreds
        if(num <= 999)
            return num ;
        
        // thousands
        else if(num >= 1000 && num <= 999999)
            return (num / 1000).toFixed(1) + 'K';
        
        // millions
        else if(num >= 1000000 && num <= 999999999)
            return (num / 1000000).toFixed(1) + 'M';
        
        // billions
        else if(num >= 1000000000 && num <= 999999999999)
            return (num / 1000000000).toFixed(1) + 'B';
        
        else
            return num ;
        }
      
}