import { Controller } from 'react-hook-form';
import { Calendar } from '@/components/ui/calendar';

function DateFormField({ control, name }: any) {
  return (
    <Controller
      name={name}
      control={control}
      render={({ field }) => (
        <Calendar
          // The selected date value comes directly from the RHF field object
          // This value must be a Date object for the calendar to highlight it.
          mode="single"
          selected={field.value} 
          onSelect={field.onChange}
          initialFocus
        />
      )}
    />
  );
}