import { useForm } from "react-hook-form";
import { Input } from "../components/ui/input";
import { Button } from "../components/ui/button";

interface LoginData {
  username: string;
  password: string;
}

export default function Login() {
  const { register, handleSubmit } = useForm<LoginData>();

  const login = (data: LoginData) => {
    const request = new Request("http://localhost:5250/api/auth", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(data),
    });

    fetch(request).then((response) => {
      if (!response.ok) {
        console.log("Bad response");
        return;
      }

      response.json().then(console.log);
    });
  };

  return (
    <form className="flex flex-col gap-5" onSubmit={handleSubmit(login)}>
      <Input type="name" {...register("username")} placeholder="Username" />
      <Input type="password" {...register("password")} placeholder="Password" />
      <Button type="submit" className="cursor-pointer">
        Login
      </Button>
    </form>
  );
}
