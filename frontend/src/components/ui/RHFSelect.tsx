// RHFSelect.tsx

import { Controller, useFormContext } from "react-hook-form";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";

interface SelectOption {
  value: string;
  label: string;
}

interface RHFSelectProps {
  name: string;
  label: string;
  placeholder?: string;
  disabled?: boolean;
  value?: string;
  options: SelectOption[];
}

export const RHFSelect = ({
  name,
  placeholder = "Select an option",
  options,
  disabled,
  value,
}: RHFSelectProps) => {
  const { control } = useFormContext();

  return (
    <Controller
      name={name}
      control={control}
      render={({ field }) => (
        <Select
          onValueChange={field.onChange}
          onOpenChange={field.onBlur}
          value={value || field.value || ""}
          disabled={disabled}
        >
          <SelectTrigger>
            <SelectValue placeholder={placeholder} />
          </SelectTrigger>
          <SelectContent>
            {options.map((option) => (
              <SelectItem key={option.value} value={option.value}>
                {option.label}
              </SelectItem>
            ))}
          </SelectContent>
        </Select>
      )}
    />
  );
};
