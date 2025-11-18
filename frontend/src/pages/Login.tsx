import { useForm } from "react-hook-form";
import { Input } from "../components/ui/input";
import { Button } from "../components/ui/button";
import axios, { HttpStatusCode, type AxiosResponse } from "axios";
import { useNavigate } from "react-router-dom";

interface LoginData {
  username: string;
  password: string;
}

export default function Login() {
  const { register, handleSubmit } = useForm<LoginData>();
  const navigate = useNavigate();

  const login = (data: LoginData) => {
    axios
      .post<any, AxiosResponse<string>>("http://localhost:5250/api/auth", data)
      .then((response) => {
        if (response.status === HttpStatusCode.Ok) {
          console.log(response.data);
          navigate("/dashboard");
        }
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
